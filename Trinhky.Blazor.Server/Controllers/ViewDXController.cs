using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Markdig;
using Microsoft.JSInterop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Trinhky.Blazor.Server.Services;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ViewDXDetailView : ViewController
    {
        private readonly SimpleAction GuiDX;
        private readonly SimpleAction DuyetDX;
        private readonly SimpleAction editPhieu;
        private readonly SimpleAction Dinhkem;
        private readonly SimpleAction GuiYKien;
        private readonly SimpleAction ThuchienDX;
        private readonly SimpleAction ChiaseDX;
        private readonly SimpleAction GiaoDX;
        private readonly SimpleAction XongAction;
        private DuyetDexuat CurrentBuocDuyet = null;
        private Phieudexuat CurPhieu;
        private Nhansu curNV;
        public ViewDXDetailView()
        {
            TargetViewId = "viewDX";

            editPhieu = new(this, "editPhieu2", "View")
            {
                Caption = "Sửa",
                ImageName = "chinhsua",
                TargetViewId = "viewDX",
                ToolTip = "Cập nhật nội dung đề xuất"
            };
            editPhieu.Execute += EditPhieu_Execute;

            Dinhkem = new(this, "Dinhkem", "View")
            {
                Caption = "Đính kèm",
                ImageName = "dinhkem",
                TargetViewId = "viewDX",
                ToolTip = "Đính kèm tài liệu"
            };
            Dinhkem.Execute += Dinhkem_Execute;

            GuiYKien = new(this, "GuiYKien", "View")
            {
                Caption = "Ý kiến",
                ImageName = "choxuly",
                TargetViewId = "viewDX",
                ToolTip = "Gửi ý kiến"
            };
            GuiYKien.Execute += GuiYKien_Execute;

            DuyetDX = new(this, "DuyetDX", "View")
            {
                Caption = "Duyệt",
                ImageName = "duyet",
                ToolTip = "Duyệt đề xuất",
                TargetViewId = "viewDX",
            };
            DuyetDX.Execute += DuyetDX_Execute;

            GiaoDX = new(this, "GiaoDX", "View")
            {
                Caption = "Giao việc",
                ImageName = "giaoviec",
                ToolTip = "Giao thực hiện đề xuất",
                TargetViewId = "viewDX",
            };
            GiaoDX.Execute += GiaoDX_Execute;

            ThuchienDX = new(this, "ThuchienDX2", "View")
            {
                Caption = "Thực hiện",
                ImageName = "work",
                ToolTip = "Lập đề xuất thực hiện",
                TargetViewId = "viewDX",
            };
            ThuchienDX.Execute += ThuchienDX_Execute;

            XongAction = new(this, "XongAction", "View")
            {
                Caption = "Xử lý",
                ImageName = "button-ok",
                TargetViewId = "viewDX",
                ToolTip = "Xác nhận đề xuất đã được thực hiện hoặc chưa"
            };
            XongAction.Execute += XongAction_Execute;

            ChiaseDX = new(this, "ChiaseDX", "View")
            {
                Caption = "Chia sẻ",
                ImageName = "share",
                ToolTip = "Chia sẻ đề xuất",
                TargetViewId = "viewDX",
            };
            ChiaseDX.Execute += ChiaseDX_Execute;

            GuiDX = new(this, "GuiDX", "View")
            {
                Caption = "Gửi",
                ImageName = "gui",
                TargetViewId = "viewDX",
                ToolTip = "Gửi hoặc hủy đề xuất"
            };
            GuiDX.Execute += GuiDX_Execute;

            SimpleAction Quaylai = new(this, "Quaylai1", "View")
            {
                Caption = "Quay lại",
                ImageName = "back",
                TargetViewId = "viewDX",
                ToolTip = "Quay lại trang trước"
            };
            Quaylai.Execute += Quaylai_Execute;
        }

        private void XongAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat phieu = objectSpace.GetObject<Phieudexuat>(CurPhieu);
                if (phieu != null)
                {
                    DetailView dView = Application.CreateDetailView(objectSpace, "XulyPhieudexuat", true, phieu);
                    Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Xuly(objectSpace));
                }
            }
        }
        private void Xuly(IObjectSpace objectSpace)
        {
            objectSpace.CommitChanges();
            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("refreshPage"));
        }

        private void GiaoDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat pmoi = objectSpace.GetObject<Phieudexuat>(CurPhieu);
                DetailView dView = Application.CreateDetailView(objectSpace, "GiaoViec", true, pmoi);
                Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => PGiaoviec(objectSpace, pmoi), null, "Gửi");
                //e.ShowViewParameters.CreatedView = dView;
                //e.ShowViewParameters.Context = TemplateContext.View;
                //e.ShowViewParameters.TargetWindow = TargetWindow.Default;
            }
        }

        private void PGiaoviec(IObjectSpace objectSpace, Phieudexuat phieudexuat)
        {
            objectSpace.CommitChanges();
            TCom.SendEmailGiaoviec(phieudexuat);

            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("refreshPage"));
        }
        private void Quaylai_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("navigateBack"));
        }

        private void ChiaseDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null && curNV != null)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ChiaseList));
                Session session = ((XPObjectSpace)ObjectSpace).Session;
                ChiaseList pmoi = objectSpace.CreateObject<ChiaseList>();
                //pmoi.Oid = Guid.NewGuid();
                pmoi.PhieuOid = CurPhieu.Oid;
                pmoi.NhanvienOid = curNV.Oid;
                //pmoi.DanhsachNVs.Clear();
                XPCollection<Nhansu> ds = new(session)
                {
                    //Criteria=CriteriaOperator.Parse("ChucvuNVs.Count>0"),
                    Sorting = new SortingCollection(new SortProperty("Email", SortingDirection.Ascending))
                };
                foreach (Nhansu item in ds)
                {
                    DanhsachNV obj = new()
                    {
                        Hoten = item.HoTen,
                        Chucvu = item.ListCV,
                        Oid = item.Oid
                    };
                    if (item.Donvi != null)
                        obj.Bophan = item.Donvi.Name;
                    if (item.CS != null)
                        obj.Coso = item.CS.TenCoso;
                    pmoi.DanhsachNVs.Add(obj);
                }
                DetailView dView = Application.CreateDetailView(objectSpace, "ChiaseList_DetailView", true, pmoi);
                e.ShowViewParameters.CreatedView = dView;
                //Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Taochiase(pmoi, objectSpace));

            }
        }

        private void ThuchienDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                string sql = "select count(Oid) from Phieudexuat where LinkID=" + CurPhieu.Oid;
                Session session = ((XPObjectSpace)ObjectSpace).Session;
                var ret = session.ExecuteScalar(sql);
                int lan = 0;
                if (ret != null)
                    lan = tmLib.CommonLib.CInt(ret);
                if (lan > 0)
                {
                    string msg = "Đề xuất này đã thực hiện " + lan + " lần. Bạn vẫn muốn thực hiện tiếp?";
                    NonPersistentObjectSpace nonPersistentObjectSpace =
                        (NonPersistentObjectSpace)Application.CreateObjectSpace(typeof(ConfirmationWindowParameters));
                    ConfirmationWindowParameters parameters = new()
                    {
                        ConfirmationMessage = msg
                    };
                    DetailView confirmationDetailView = Application.CreateDetailView(nonPersistentObjectSpace, parameters);
                    confirmationDetailView.Caption = "Xác nhận";
                    Application.ShowViewStrategy.ShowViewInPopupWindow(confirmationDetailView, LapDXThuchien);

                }
                else
                    LapDXThuchien();
            }
        }
        private void LapDXThuchien()
        {
            if (CurPhieu != null)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat pmoi = objectSpace.CreateObject<Phieudexuat>();
                pmoi.LinkID = CurPhieu.Oid;
                pmoi.Thuchien = true;
                pmoi.TenDexuat = "Thực hiện đề xuất số " + CurPhieu.Oid;
                DetailView dView = Application.CreateDetailView(objectSpace, "Phieumoi", true, pmoi);
                Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Lapphieu(pmoi, objectSpace));
            }

        }
        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        void Lapphieu(Phieudexuat phieu, IObjectSpace objectSpace)
        {
            if (TCom.LapphieuDX(phieu, objectSpace))
            {
                IObjectSpace objectSpace1 = Application.CreateObjectSpace(typeof(Phieudexuat));
                Phieudexuat p = objectSpace1.GetObject(phieu);
                TCom.AddUserValue("viewdx", p.Oid.ToString());
                DashboardView dashboardView = Application.CreateDashboardView(objectSpace1, "viewDX", true);
                ShowViewParameters svp = new()
                {
                    CreatedView = dashboardView,
                    TargetWindow = TargetWindow.Default,
                    Context = TemplateContext.View
                };
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
            }
        }
        private void GuiDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                Phieudexuat p = ObjectSpace.GetObject(CurPhieu);
                if (p != null)
                {
                    if (p.Trangthai == TCom.EStatusVB.choduyet)
                    {
                        string msg = (p.Dagui ? "Chắc chắn thu hồi đề xuất này?" : "Chắc chắn gửi đề xuất này?");
                        NonPersistentObjectSpace objectSpace =
                            (NonPersistentObjectSpace)Application.CreateObjectSpace(typeof(ConfirmationWindowParameters));
                        ConfirmationWindowParameters parameters = new()
                        {
                            ConfirmationMessage = msg
                        };
                        DetailView confirmationDetailView = Application.CreateDetailView(objectSpace, parameters);
                        confirmationDetailView.Caption = "Xác nhận";
                        Application.ShowViewStrategy.ShowViewInPopupWindow(confirmationDetailView, OkDelegate,null, "Đồng ý");
                    }
                    else
                        TCom.CustomError("Đề xuất đang được xử lý");
                }
            }
        }
        private void OkDelegate()
        {
            CurPhieu.Dagui = !CurPhieu.Dagui;
            ObjectSpace.CommitChanges();
            string msg = (CurPhieu.Dagui ? "Bạn đã gửi đề xuất" : "Bạn đã hủy đề xuất");
            Application.ShowViewStrategy.ShowMessage(msg);
            TCom.SendEmailDexuat(CurPhieu);
            //Chia se phieu neu duoc giao thuc hien
            if (CurPhieu.LinkID > 0 && CurPhieu.Dagui)
            {
                Phieudexuat phieucu = ObjectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", CurPhieu.LinkID));
                if (phieucu != null)
                {
                    if (phieucu.NguoiXuly != null && phieucu.Nguoithuchien != null)
                    {
                        if (phieucu.Nguoithuchien.Oid == curNV.Oid)//nguoi thuc hien lap phieu
                        {
                            Chiase chiase = ObjectSpace.CreateObject<Chiase>();
                            chiase.Phieu = CurPhieu;
                            chiase.NhanvienShare = ObjectSpace.GetObject(curNV);
                            chiase.NhanvienShared = phieucu.NguoiXuly;
                            chiase.Ykien = "Thực hiện đề xuất số " + CurPhieu.LinkID.ToString();
                            chiase.Save();
                            ObjectSpace.CommitChanges();
                            string uMail = (string.IsNullOrEmpty(TCom.testMail) ? phieucu.NguoiXuly.Email : TCom.testMail);
                            TCom.SendEmailChiase(CurPhieu, uMail);
                        }
                    }
                }
            }

            if (CurPhieu.Dagui)
                GuiDX.Caption = "Hủy";
            else
                GuiDX.Caption = "Gửi";

            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("refreshPage"));

        }
        private void GuiYKien_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                if (CurPhieu.Trangthai == TCom.EStatusVB.dahuy)
                {
                    TCom.CustomError("Văn bản đã bị hủy");
                }
                else if (CurPhieu.Trangthai == TCom.EStatusVB.daduyet)
                {
                    TCom.CustomError("Văn bản đã duyệt");
                }
                else
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Quatrinh));
                    Quatrinh quatrinh = objectSpace.CreateObject<Quatrinh>();
                    Nhansu nv = TCom.GetCurrentNhanvien();
                    quatrinh.Phieu = objectSpace.GetObject(CurPhieu);
                    quatrinh.Nhanvien = objectSpace.GetObject(nv);
                    DetailView dView = Application.CreateDetailView(objectSpace, "YKienMoi", true, quatrinh);
                    Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => YkienSave(objectSpace, quatrinh), null, "Gửi");
                    //e.ShowViewParameters.CreatedView = dView;
                }
            }
        }
        private void YkienSave(IObjectSpace objectSpace, Quatrinh yk)
        {
            yk.Save();
            objectSpace.CommitChanges();
            TCom.SendEmailYkien(yk);

            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("refreshPage"));

        }
        private void Dinhkem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                if (CurPhieu.Trangthai == TCom.EStatusVB.dahuy)
                {
                    TCom.CustomError("Văn bản đã bị hủy");
                }
                else if (CurPhieu.Trangthai == TCom.EStatusVB.daduyet)
                {
                    TCom.CustomError("Văn bản đã duyệt");
                }
                else
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(TepDinhkem));
                    DashboardView dView = Application.CreateDashboardView(objectSpace, "upload", true);
                    e.ShowViewParameters.CreatedView = dView;
                }
            }
        }
        private void EditPhieu_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurPhieu != null)
            {
                //BCom.CreateLog(CurPhieu);
                //return;
                if (CurPhieu.Trangthai == TCom.EStatusVB.daduyet)
                {
                    TCom.CustomError("Phiếu đã duyệt, không sửa nội dung được");
                }
                else if (CurPhieu.Trangthai == TCom.EStatusVB.dahuy)
                {
                    TCom.CustomError("Phiếu đã hủy. Bạn không sửa nội dung được");
                }
                else if (CurPhieu.Trangthai == TCom.EStatusVB.dangduyet)
                {
                    if (CurrentBuocDuyet == null)
                    {
                        TCom.CustomError("Đề xuất đang duyệt, Bạn không sửa nội dung được");
                    }
                    else
                    {
                        if (CurrentBuocDuyet.Nguoiduyet.Oid == curNV.Oid)
                            if (CurrentBuocDuyet.Trangthai == TCom.EStatusDuyet.daduyet)
                                TCom.CustomError("Bạn đã duyệt, không sửa nội dung được");
                            else if (CurrentBuocDuyet.Trangthai == TCom.EStatusDuyet.huy)
                                TCom.CustomError("Bạn đã hủy, không sửa nội dung được");
                            else
                            {
                                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                                DetailView dView = Application.CreateDetailView(objectSpace, "Noidungphieu", true, objectSpace.GetObject(CurPhieu));
                                e.ShowViewParameters.CreatedView = dView;
                            }
                    }
                }
                else//cho duyet
                {
                    if(CurPhieu.Dagui && curNV.Oid == CurPhieu.Nhanvien.Oid)
                    {
                        TCom.CustomError("Đề xuất đã gửi đi, Bạn không sửa nội dung được");
                    }
                    else
                    {
                        IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Phieudexuat));
                        //DetailView dView = Application.CreateDetailView(objectSpace, "Phieudexuat_DetailView", true, objectSpace.GetObject(CurPhieu));
                        DetailView dView = Application.CreateDetailView(objectSpace, "Noidungphieu", true, objectSpace.GetObject(CurPhieu));
                        e.ShowViewParameters.CreatedView = dView;
                    }
                }
            }
        }

        private void DuyetDX_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                if (CurPhieu.Dagui == false)
                {
                    TCom.CustomError("Đề xuất chưa gửi đi, Bạn không duyệt được");
                    return;
                }
                if (curNV != null && CurrentBuocDuyet != null)
                {
                    IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(DuyetDexuat));
                    DuyetDexuat duyet = objectSpace.GetObject(CurrentBuocDuyet);
                    duyet.DuyetUQ = false;
                    if (duyet.NguoiUQ != null)
                    {
                        if (duyet.NguoiUQ.Oid == curNV.Oid)
                            duyet.DuyetUQ = true;
                    }
                    DetailView dView = Application.CreateDetailView(objectSpace, "DuyetView", true, duyet);
                    Application.ShowViewStrategy.ShowViewInPopupWindow(dView, () => Duyet(objectSpace, duyet), null, "Gửi");
                    //e.ShowViewParameters.CreatedView = dView;
                    //e.ShowViewParameters.Context = TemplateContext.View;
                    //e.ShowViewParameters.TargetWindow = TargetWindow.Default;
                }
            }
            catch (Exception ex)
            {
                TCom.CustomError(ex.Message);
            }
        }

        void Duyet(IObjectSpace objectSpace, DuyetDexuat buocduyet)
        {
            buocduyet.Ngayduyet = TCom.GetServerDateTime();
            buocduyet.Save();
            objectSpace.CommitChanges();
            //kiem tra neu ket thuc
            if (buocduyet.Ketthuc)
            {
                foreach(DuyetDexuat buoc in buocduyet.Phieu.DuyetDexuats)
                {
                    if (buoc.SoTT > buocduyet.SoTT)
                    {
                        buoc.Boqua = true;
                        buoc.MaKythay = "{UQTGD}";
                    }
                }
                objectSpace.CommitChanges();
            }
            BCom.UpdateTrangthaiPhieu(buocduyet.Phieu.Oid);
            TCom.SendEmailDuyet(buocduyet);
            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("refreshPage"));

            //Controller controller = Frame.GetController<WarningController>();
            //if (controller != null)
            //{
            //    var act = controller.Actions["ListChoduyet"];
            //    if (act != null)
            //    {
            //        string info = TCom.Sochoduyet();
            //        if (string.IsNullOrEmpty(info))
            //            act.Active.SetItemValue("thuong", false);
            //        else
            //            act.Caption = "Chờ duyệt (" + info + ")";
            //    }
            //}

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            bool DcDuyet = false;
            curNV = TCom.GetCurrentNhanvien();

            int phieuid = TCom.GetUserIntValue("viewdx");
            if (phieuid > 0)
            {
                CurPhieu = ObjectSpace.FindObject<Phieudexuat>(CriteriaOperator.Parse("Oid=?", phieuid));
                if (CurPhieu != null && curNV != null)
                {
                    bool CoChucvu = true;
                    //if (curNV.ChucvuNVs.Count > 0) CoChucvu = true;

                    if (CurPhieu.Trangthai == TCom.EStatusVB.dahuy)
                    {
                        GuiDX.Active.SetItemValue("an", false);
                        editPhieu.Active.SetItemValue("an", false);
                        Dinhkem.Active.SetItemValue("an", false);
                        GuiYKien.Active.SetItemValue("an", false);
                        DuyetDX.Active.SetItemValue("an", false);
                        ChiaseDX.Active.SetItemValue("an", false);
                        ThuchienDX.Active.SetItemValue("an", false);
                        GiaoDX.Active.SetItemValue("an", false);
                        XongAction.Active.SetItemValue("an", false);
                    }
                    else if (CurPhieu.Trangthai == TCom.EStatusVB.daduyet)
                    {
                        GuiDX.Active.SetItemValue("an", false);
                        editPhieu.Active.SetItemValue("an", false);
                        Dinhkem.Active.SetItemValue("an", false);
                        GuiYKien.Active.SetItemValue("an", false);
                        DuyetDX.Active.SetItemValue("an", false);

                        ChiaseDX.Active.SetItemValue("an", CoChucvu);
                        //xu ly
                        bool bXuly = false;
                        bool bGiaoviec = false;
                        if (CurPhieu.Trangthai == TCom.EStatusVB.daduyet)
                        {
                            if (CurPhieu.NguoiXuly != null)
                            {
                                if (CurPhieu.NguoiXuly.Oid == curNV.Oid)
                                {
                                    bXuly = true;
                                    bGiaoviec = true;
                                }
                            }
                            if (CurPhieu.Nguoithuchien != null)
                            {
                                if (CurPhieu.Nguoithuchien.Oid == curNV.Oid)
                                {
                                    bXuly = true;
                                }
                            }
                        }
                        XongAction.Active.SetItemValue("an", bXuly);
                        ThuchienDX.Active.SetItemValue("an", bXuly);
                        GiaoDX.Active.SetItemValue("an", bGiaoviec);
                    }
                    else
                    {
                        if (CurPhieu.Trangthai == TCom.EStatusVB.choduyet && CurPhieu.Nhanvien.Oid == curNV.Oid)
                        {
                            if (CurPhieu.Dagui)
                                GuiDX.Caption = "Hủy";
                            else
                                GuiDX.Caption = "Gửi";
                            GuiDX.Active.SetItemValue("an", true);
                        }
                        else
                            GuiDX.Active.SetItemValue("an", false);

                        //Duyet
                        Session session = ((XPObjectSpace)ObjectSpace).Session;
                        string sql = @"SELECT TOP (100) PERCENT Trangthai, Nguoiduyet, OID, NguoiUQ 
                        FROM     dbo.DuyetDexuat WHERE (Trangthai=0) and  (Phieu = " + CurPhieu.Oid + ") ORDER BY SoTT";
                        SelectedData results2 = session.ExecuteQuery(sql);
                        foreach (SelectStatementResultRow row in results2.ResultSet[0].Rows)
                        {
                            int trangthai = tmLib.CommonLib.CInt(row.Values[0]);
                            int nvId = tmLib.CommonLib.CInt(row.Values[1]);
                            int duyetId = tmLib.CommonLib.CInt(row.Values[2]);
                            int nvUQ = tmLib.CommonLib.CInt(row.Values[3]);
                            if (nvId == curNV.Oid || nvUQ == curNV.Oid)
                            {
                                CurrentBuocDuyet = ObjectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("Oid=?", duyetId));
                                if (CurrentBuocDuyet != null)
                                {
                                    int sott = CurrentBuocDuyet.SoTT + 1;
                                    DuyetDexuat duyetsau = ObjectSpace.FindObject<DuyetDexuat>(CriteriaOperator.Parse("SoTT=? && Phieu=?", sott, CurPhieu));
                                    if (duyetsau == null)
                                    {
                                        DcDuyet = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (duyetsau.Trangthai != TCom.EStatusDuyet.choduyet)
                                        {
                                            DcDuyet = false;
                                        }
                                        else
                                            DcDuyet = true;

                                        break;
                                    }
                                    //break;
                                }
                            }
                        }
                        if (CurPhieu.Dagui == false)
                            DcDuyet = false;
                        DuyetDX.Active.SetItemValue("an", DcDuyet);
                        ThuchienDX.Active.SetItemValue("an", false);
                        GiaoDX.Active.SetItemValue("an", false);
                        XongAction.Active.SetItemValue("an", false);
                        //Dinh kem
                        GuiYKien.Active.SetItemValue("an", true);
                        Dinhkem.Active.SetItemValue("an", true);
                        editPhieu.Active.SetItemValue("an", true);
                        //DX chia se
                        bool DuocCS = false;
                        foreach (Chiase item in CurPhieu.Chiases)
                        {
                            if (item.NhanvienShared.Oid == curNV.Oid)
                            {
                                DuocCS = true;
                                break;
                            }
                        }
                        editPhieu.Active.SetItemValue("an", !DuocCS);
                        ChiaseDX.Active.SetItemValue("an", CoChucvu);
                    }
                }
            }

        }
    }
}
