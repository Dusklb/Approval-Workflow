using DevExpress.Blazor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Grid;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using Microsoft.JSInterop;
using System;
using System.Text.Json;
using Trinhky.Blazor.Server.Pages;

namespace Trinhky.Blazor.Server.Controllers
{
    public class BlazorListViewController : ViewController<ListView>
    {
        private string LocalStorageKey;// = "Grid-LayoutPersistence-Data";
        private readonly string chuoisum = "Sodx;Chuaduyet;Dangduyet;Huy;Daduyet;Daxong;Chuaduyet2;Dunghan;Quahan;";

        async Task SaveLayoutToLocalStorageAsync(GridPersistentLayout layout)
        {
            try
            {
                if ((BlazorApplication)Application != null)
                {
                    var json = JsonSerializer.Serialize(layout);
                    IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
                }
            }
            catch
            {
                // Mute exceptions
            }
        }
        async Task Grid_LayoutAutoSaving(GridPersistentLayoutEventArgs e)
        {
            await SaveLayoutToLocalStorageAsync(e.Layout);
        }
        async Task<GridPersistentLayout> LoadLayoutFromLocalStorageAsync()
        {
            try
            {
                IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
                var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
                return JsonSerializer.Deserialize<GridPersistentLayout>(json);
            }
            catch
            {
                // Mute exceptions
                return null;
            }
        }
        async Task Grid_LayoutAutoLoading(GridPersistentLayoutEventArgs e)
        {
            e.Layout = await LoadLayoutFromLocalStorageAsync();
            //if (LocalStorageKey != "Phiếu đề xuất" && LocalStorageKey != "Chờ duyệt")
            //{
            //    e.Layout = await LoadLayoutFromLocalStorageAsync();
            //}
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.Editor is DxGridListEditor gridListEditor)
            {
                IDxGridAdapter dataGridAdapter = gridListEditor.GetGridAdapter();
                LocalStorageKey = gridListEditor.Name;
                dataGridAdapter.GridModel.LayoutAutoLoading = Grid_LayoutAutoLoading;
                dataGridAdapter.GridModel.LayoutAutoSaving = Grid_LayoutAutoSaving;
                //dataGridAdapter.GridModel
                dataGridAdapter.GridModel.ColumnResizeMode = DevExpress.Blazor.GridColumnResizeMode.ColumnsContainer;
                foreach (DxGridDataColumnModel columnModel in dataGridAdapter.GridDataColumnModels)
                {
                    string fmt = columnModel.DisplayFormat;
                    if (!string.IsNullOrEmpty(fmt))
                    {
                        if (fmt.Contains('N') || fmt.Contains('C') || fmt.Contains('#'))
                            columnModel.CellDisplayTemplate = CellDisplayTemplate.GetNonZero();
                        //columnModel.FilterRowValueChanged
                    }
                }
                dataGridAdapter.GridModel.CustomizeSummaryDisplayText = Grid_CustomizeSummaryDisplayText;
                // gridListEditor.Model.Columns.
            }
            else if (View.Editor is GridListEditor grid)
            {
                IDxDataGridAdapter dataGridAdapter = grid.GetDataGridAdapter();
                dataGridAdapter.DataGridModel.ColumnResizeMode = DataGridColumnResizeMode.Component;
                dataGridAdapter.DataGridModel.HorizontalScrollBarMode = ScrollBarMode.Auto;
                //dataGridAdapter.DataGridModel.CssClass = "my-grid";
            }
        }
        void Grid_CustomizeSummaryDisplayText(GridCustomizeSummaryDisplayTextEventArgs e)
        {
            if (chuoisum.Contains(e.Item.FieldName + ";"))
                e.DisplayText = string.Format("{0:###.###.###.###}", e.Value);

            //if (e.Item.FieldName == "TienTruocthue" || e.Item.FieldName == "Tienthue" || e.Item.FieldName == "Tienck"
            //    || e.Item.FieldName == "Tiengiam" || e.Item.FieldName == "Thanhtien" || e.Item.FieldName == "Giavon"
            //    || e.Item.FieldName == "Laigop")
            //    e.DisplayText = string.Format(TmCom.optfmtTien, e.Value);
        }


    }

}
