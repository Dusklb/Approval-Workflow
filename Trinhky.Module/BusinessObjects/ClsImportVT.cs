using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DomainComponent]
    [System.ComponentModel.DisplayName("Cập nhật vật tư")]
    [ImageName("import")]
    public class ClsImportVT
    {
        [Browsable(false)]
        public NhomVT Nhomhang { get; set; }

        [VisibleInListView(false)]
        [XafDisplayName("Dữ liệu từ Clipboard")]
        public string Noidung { get; set; }

        private BindingList<ImportHangData> _ImportHangs;
        [XafDisplayName("Vật tư")]
        public BindingList<ImportHangData> ImportHangs
        {
            get
            {
                _ImportHangs ??= new BindingList<ImportHangData>();
                return _ImportHangs;
            }
        }
    }
}
