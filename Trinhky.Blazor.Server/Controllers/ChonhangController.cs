using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraRichEdit;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Trinhky.Blazor.Server.Services;
using Trinhky.Module.BusinessObjects;

namespace Trinhky.Blazor.Server.Controllers
{
    public class ChonhangController : ViewController
    {
        private readonly PopupWindowShowAction ChonVT;
        private readonly PopupWindowShowAction GoiYDX;
        private SimpleAction actKiemTraAI;       // Cái mới (Validation)
        public ChonhangController()
        {
            TargetViewId = "Noidungphieu";
            //ChonVT = new(this, "ChonVT", "View")
            //{
            //    Caption = "Chọn vật tư",
            //    ImageName = "products",
            //    TargetViewId = "Noidungphieu",
            //    ToolTip = "Chọn vật tư, hàng hóa từ danh mục"
            //};
            //ChonVT.CustomizePopupWindowParams += ChonVT_CustomizePopupWindowParams;
            //ChonVT.Execute += ChonVT_Execute; ;

            GoiYDX = new(this, "GoiYDX", "View")
            {
                Caption = "Gợi ý",
                ImageName = "goiy",
                TargetViewId = "Noidungphieu",
                ToolTip = "AI Gợi ý nội dung đề xuất"
            };
            GoiYDX.CustomizePopupWindowParams += GoiYDX_CustomizePopupWindowParams;
            GoiYDX.Execute += GoiYDX_Execute;

            actKiemTraAI = new(this, "actKiemTraAI", "View")
            {
                Caption = "Kiểm tra",
                ImageName = "check",
                TargetViewId = "Noidungphieu",
                ToolTip = "AI kiểm tra nội dung đề xuất"
            };
            actKiemTraAI.Execute += ActKiemTraAI_Execute;

            SimpleAction Quaylai = new(this, "Quaylai2", "View")
            {
                Caption = "Quay lại",
                ImageName = "back",
                TargetViewId = "Noidungphieu",
                ToolTip = "Quay lại trang trước"
            };
            Quaylai.Execute += Quaylai_Execute;
        }

        private async void ActKiemTraAI_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                // 1. Lấy đối tượng hiện tại
                var curPhieu = e.CurrentObject as Phieudexuat; // <-- Thay bằng class của bạn (VD: PhieuDeXuat)
                if (curPhieu == null) return;

                // 2. Chuyển đổi dữ liệu từ DxRichEdit sang Text thuần
                // Thay 'curPhieu.NoiDungDeXuat' bằng tên property RichText thực tế của bạn
                string contentToCheck = GetTextFromRichEditData(curPhieu.Noidung);

                // Kiểm tra nếu nội dung rỗng
                if (string.IsNullOrWhiteSpace(contentToCheck) || contentToCheck.Length < 10)
                {
                    Application.ShowViewStrategy.ShowMessage("Nội dung quá ngắn hoặc trống, không thể thẩm định!", InformationType.Warning);
                    return;
                }

                // 3. Thông báo người dùng đợi
                Application.ShowViewStrategy.ShowMessage("AI đang đóng vai 'Thanh tra' để rà soát lỗi...", InformationType.Info);

                var geminiService = Application.ServiceProvider.GetRequiredService<GeminiService>();

                // 4. XÂY DỰNG PROMPT CHI TIẾT (Role-play Audit)
                // Kỹ thuật: Yêu cầu AI trả về HTML với class CSS cụ thể để ta định dạng sau này
                string prompt =
                    $"Đóng vai trò là một Chuyên gia Kiểm duyệt văn bản (Senior Auditor) khó tính. \n" +
                    $"Nhiệm vụ: Thẩm định văn bản đề xuất dưới đây. \n\n" +
                    $"--- NỘI DUNG CẦN KIỂM TRA ---\n" +
                    $"{contentToCheck}\n" +
                    $"--- HẾT NỘI DUNG ---\n\n" +

                    $"Tiêu chí chấm điểm: \n" +
                    $"1. Chính tả & Ngữ pháp (Tiếng Việt). \n" +
                    $"2. Văn phong (Phải trang trọng, hành chính, chuyên nghiệp). \n" +
                    $"3. Tính logic & Đầy đủ (Có bị lủng củng hay thiếu thông tin quan trọng không). \n\n" +

                    $"YÊU CẦU ĐỊNH DẠNG OUTPUT (BẮT BUỘC TRẢ VỀ HTML): \n" +
                    $"Hãy trình bày kết quả dưới dạng một Báo cáo HTML (HTML Fragment), sử dụng đúng các class CSS sau: \n" +

                    $"- Phần 1: KẾT LUẬN CHUNG. \n" +
                    $"  + Nếu Tốt: Dùng thẻ <div class='status-box status-pass'>ĐÁNH GIÁ: ĐẠT YÊU CẦU</div> \n" +
                    $"  + Nếu Tạm được (cần sửa nhẹ): Dùng thẻ <div class='status-box status-warning'>ĐÁNH GIÁ: CẦN CHỈNH SỬA</div> \n" +
                    $"  + Nếu Tệ: Dùng thẻ <div class='status-box status-fail'>ĐÁNH GIÁ: KHÔNG ĐẠT</div> \n\n" +

                    $"- Phần 2: CHI TIẾT LỖI (Liệt kê các lỗi tìm thấy). \n" +
                    $"  Mỗi lỗi để trong thẻ: <div class='issue-item'>[Lỗi] ...lý do...</div> \n\n" +

                    $"- Phần 3: GỢI Ý SỬA ĐỔI (Viết lại câu văn cho hay hơn). \n" +
                    $"  Mỗi gợi ý để trong thẻ: <div class='suggestion-item'>[Gợi ý] ...nội dung...</div> \n\n" +

                    $"Lưu ý: Chỉ trả về mã HTML nằm trong thẻ div, không trả về thẻ <html>, <head>, <body> hay markdown ```html.";

                // 5. Gọi AI
                string rawResponse = await geminiService.GenerateTextAsync(prompt);

                // 6. Làm sạch và hiển thị Popup
                string finalHtml = CleanAIResponse(rawResponse, isValidation: true);
                ShowResultPopup(finalHtml);
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Lỗi khi thẩm định: {ex.Message}", InformationType.Error);
            }
        }

        // --- Helper 1: Chuyển đổi DxRichEdit Data -> Plain Text ---
        private string GetTextFromRichEditData(object richData)
        {
            if (richData == null) return "";

            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                // Xử lý nếu lưu dạng byte[] (Docx, OpenXml) - Mặc định của XAF Office
                if (richData is byte[] bytes)
                {
                    if (bytes.Length == 0) return "";
                    docServer.LoadDocument(new MemoryStream(bytes), DocumentFormat.OpenXml);
                }
                // Xử lý nếu lưu dạng String (HTML hoặc RTF)
                else if (richData is string strData)
                {
                    if (string.IsNullOrWhiteSpace(strData)) return "";
                    if (strData.Trim().StartsWith("<"))
                        docServer.HtmlText = strData;
                    else
                        docServer.RtfText = strData;
                }

                // Trả về Text thuần để AI dễ đọc và tiết kiệm Token
                return docServer.Text;
            }
        }

        // --- Helper 2: Làm sạch Response & Inject CSS ---
        private string CleanAIResponse(string response, bool isValidation = false)
        {
            if (string.IsNullOrEmpty(response)) return "";

            // Bóc tách Markdown block nếu có
            var match = Regex.Match(response, @"```(?:html)?\s*([\s\S]*?)\s*```");
            if (match.Success) response = match.Groups[1].Value;

            // Xóa thẻ rác
            response = response.Replace("<!DOCTYPE html>", "").Replace("<html>", "").Replace("<body>", "").Replace("</body>", "");

            // CSS Style (Đã bao gồm style cho Validation)
            string css = @"
        <style>
            .ai-wrapper { font-family: 'Segoe UI', Arial, sans-serif; font-size: 14px; line-height: 1.6; color: #333; }
            
            /* Style cho Bảng kết quả (Gợi ý) */
            .ai-wrapper h3 { color: #007bff; border-bottom: 2px solid #eee; padding-bottom: 5px; margin-top: 15px; }
            .ai-wrapper table { width: 100%; border-collapse: collapse; margin: 10px 0; }
            .ai-wrapper th, .ai-wrapper td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            .ai-wrapper th { background-color: #f8f9fa; font-weight: 600; }

            /* Style cho Validation (Thẩm định) */
            .status-box { 
                padding: 15px; border-radius: 6px; color: white; 
                margin-bottom: 20px; font-weight: bold; text-align: center; font-size: 16px; text-transform: uppercase;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            }
            .status-pass { background-color: #28a745; }    /* Xanh lá */
            .status-warning { background-color: #ffc107; color: #333; } /* Vàng cam */
            .status-fail { background-color: #dc3545; }    /* Đỏ */

            .issue-item { 
                background-color: #fff5f5; border-left: 5px solid #dc3545;
                padding: 10px; margin-bottom: 10px; border-radius: 4px;
            }
            .suggestion-item { 
                background-color: #f0fff4; border-left: 5px solid #28a745;
                padding: 10px; margin-bottom: 10px; border-radius: 4px;
            }
        </style>
        <div class='ai-wrapper'>";

            return css + response + "</div>";
        }
        private void ChonVT_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0 && e.CurrentObject is Phieudexuat phieu)
            {
                var richServer = new RichEditDocumentServer();
                richServer.Document.RtfText = phieu.NoidungGoc;
                int i = 0;
                foreach (Vattu vattu in e.PopupWindowViewSelectedObjects)
                {
                    i += 1;
                    string tenvt = vattu.Tenvt;
                    string dvt = vattu.Dvt;
                    string mavt = vattu.MaVT;
                    string giavt = vattu.Dongia.ToString("###.###.###");
                    string tt = tmLib.CommonLib.Right("0" + i.ToString(), 2);

                    if (string.IsNullOrEmpty(tenvt)) tenvt = "";
                        richServer.Document.ReplaceAll("hh" + tt, tenvt, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (string.IsNullOrEmpty(dvt)) dvt = "";
                        richServer.Document.ReplaceAll("dvt" + tt, dvt, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (string.IsNullOrEmpty(mavt)) mavt = "";
                        richServer.Document.ReplaceAll("ma" + tt, mavt, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    if (string.IsNullOrEmpty(giavt)) giavt = "";
                        richServer.Document.ReplaceAll("gia" + tt, giavt, DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                //xoa thua
                for(int j = i; j < 100; j++)
                {
                    string tt = tmLib.CommonLib.Right("0" + j.ToString(), 2);
                    richServer.Document.ReplaceAll("hh" + tt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("dvt" + tt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("ma" + tt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                    richServer.Document.ReplaceAll("gia" + tt, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
                }
                phieu.Noidung = richServer.Document.RtfText;
                phieu.Save();
                ObjectSpace.CommitChanges();
            }
        }

        private void ChonVT_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Vattu));

            CollectionSourceBase newCollectionSource = new CollectionSource(objectSpace, typeof(Vattu));
            ListView lView = Application.CreateListView("Vattu_LookupListView", newCollectionSource, true);
            e.View = lView;

            //string listViewId = Application.FindLookupListViewId(typeof(Vattu));
            //CollectionSourceBase collectionSource = Application.CreateCollectionSource(
            //    objectSpace, typeof(Vattu), listViewId);
            //IModelListView modelListView = (IModelListView)Application.FindModelView(listViewId);
            //ListView lView = Application.CreateListView(modelListView, collectionSource, false);
            //lView.Model.Filter = null;
            //lView.Model.Criteria = null;
            //lView.SaveModel();
            //lView = Application.CreateListView(modelListView, collectionSource,true);
            //e.View = lView;// Application.CreateListView(modelListView, collectionSource, true);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            bool chon = false;
            Nhansu nv = TCom.GetCurrentNhanvien();
            if (View.CurrentObject is Phieudexuat p && nv!=null)
            {
                if (p.Mau != null)
                {
                    if(p.Mau.ChonVT && nv.Oid==p.Nhanvien.Oid && p.Dagui==false)
                    {
                        chon = true;
                    }
                }
            }
            ChonVT?.Active.SetItemValue("an", chon);
        }
        private void Quaylai_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IJSRuntime jsRuntime = (IJSRuntime)((BlazorApplication)Application).ServiceProvider.GetService(typeof(IJSRuntime));
            System.Threading.Tasks.Task.Run(async () => await jsRuntime.InvokeAsync<object>("navigateBack"));
        }

        private void GoiYDX_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace(typeof(AIInputParam));
            var inputParam = new AIInputParam();

            // Có thể set giá trị mặc định nếu muốn
            inputParam.YeuCauChiTiet = "Viết chi tiết, đầy đủ các mục, văn phong chuyên nghiệp.";

            DetailView dv = Application.CreateDetailView(os, inputParam);
            dv.ViewEditMode = ViewEditMode.Edit;

            e.View = dv;
            e.Context = TemplateContext.PopupWindow;
        }
        private async void GoiYDX_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                var curPhieu = e.CurrentObject as Phieudexuat;
                var inputParam = e.PopupWindowViewCurrentObject as AIInputParam; // Lấy dữ liệu người dùng vừa nhập

                if (curPhieu == null) return;

                string userInstruction = inputParam?.YeuCauChiTiet ?? "";

                Application.ShowViewStrategy.ShowMessage("AI đang suy nghĩ...", InformationType.Info);

                var geminiService = Application.ServiceProvider.GetRequiredService<GeminiService>();

                // --- TẠO PROMPT KẾT HỢP ---
                string prompt = $"Bạn là trợ lý ảo soạn thảo văn bản hành chính/kỹ thuật. \n" +
                                $"Nhiệm vụ: Viết nội dung cho đề xuất có tên: '{curPhieu.TenDexuat}'. \n" +
                                $"Yêu cầu đặc biệt từ người dùng: '{userInstruction}'. \n\n" + // <--- Đưa yêu cầu vào đây
                                $"Quy định format: \n" +
                                $"1. Trả về HTML Fragment (trong thẻ div). \n" +
                                $"2. Dùng thẻ <h3>, <p>, <ul>, <li>, <table> (border='1'). \n" +
                                $"3. KHÔNG trả về <html>, <head>, <body>. \n" +
                                $"4. Chỉ trả về code, không chào hỏi.";

                string rawResponse = await geminiService.GenerateTextAsync(prompt);
                string finalHtml = CleanAIResponse(rawResponse);

                // Hiển thị kết quả (Popup thứ 2)
                ShowResultPopup(finalHtml);
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Lỗi: {ex.Message}", InformationType.Error);
            }
        }
        private string CleanAIResponse(string response)
        {
            if (string.IsNullOrEmpty(response)) return "";

            // Bước 1: Nếu AI trả về markdown code block (```html ... ```), hãy bóc tách nó ra
            // Regex này tìm nội dung nằm giữa ```html và ``` hoặc giữa ``` và ```
            var match = Regex.Match(response, @"```(?:html)?\s*([\s\S]*?)\s*```");
            if (match.Success)
            {
                response = match.Groups[1].Value;
            }

            // Bước 2: Xóa các thẻ thừa nếu AI lỡ đưa vào (doctype, html, body)
            response = response.Replace("<!DOCTYPE html>", "", StringComparison.OrdinalIgnoreCase);
            response = response.Replace("<html>", "", StringComparison.OrdinalIgnoreCase);
            response = response.Replace("</html>", "", StringComparison.OrdinalIgnoreCase);
            response = response.Replace("<body>", "", StringComparison.OrdinalIgnoreCase);
            response = response.Replace("</body>", "", StringComparison.OrdinalIgnoreCase);

            // Bước 3: Thêm CSS cơ bản để bảng đẹp hơn (vì HTML trần rất xấu)
            string css = @"
                <style>
                    .ai-content { font-family: 'Segoe UI', sans-serif; line-height: 1.6; }
                    .ai-content h3 { color: #2c3e50; border-bottom: 2px solid #eee; padding-bottom: 5px; margin-top: 20px; }
                    .ai-content table { width: 100%; border-collapse: collapse; margin: 15px 0; }
                    .ai-content th, .ai-content td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    .ai-content th { background-color: #f2f2f2; font-weight: bold; }
                    .ai-content ul { padding-left: 20px; }
                </style>
                <div class='ai-content'>";

            return css + response + "</div>";
        }

        private void ShowResultPopup(string htmlContent)
        {
            IObjectSpace os = Application.CreateObjectSpace(typeof(AIResultPopup));
            var popupObj = new AIResultPopup(htmlContent);
            DetailView popupView = Application.CreateDetailView(os, popupObj);
            popupView.ViewEditMode = ViewEditMode.View;

            ShowViewParameters svp = new ShowViewParameters();
            svp.CreatedView = popupView;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            svp.Context = TemplateContext.PopupWindow;

            DialogController dc = Application.CreateController<DialogController>();
            dc.AcceptAction.Caption = "Đóng";
            svp.Controllers.Add(dc);

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Application.MainWindow, null));
        }
    }
}
