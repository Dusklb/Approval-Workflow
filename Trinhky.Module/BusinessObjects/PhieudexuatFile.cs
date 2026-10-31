using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.Linq;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ImageName("dinhkem")]
    [System.ComponentModel.DisplayName("Tài liệu kèm theo")]
    [NavigationItem(false)]
    [DefaultProperty("Tailieu")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class PhieudexuatFile : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public PhieudexuatFile(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            if (Session.IsNewObject(this))
                Ngay = TCom.GetServerDateTime();

        }
        private Phieudexuat _Phieu;
        [Association]
        [XafDisplayName("Phiếu đề xuất")]
        public Phieudexuat Phieu
        {
            get { return _Phieu; }
            set { SetPropertyValue<Phieudexuat>(nameof(Phieu), ref _Phieu, value); }
        }

        private Nhansu _Nhanvien;
        [Association]
        [XafDisplayName("Người gửi")]
        public Nhansu Nhanvien
        {
            get { return _Nhanvien; }
            set { SetPropertyValue<Nhansu>(nameof(Nhanvien), ref _Nhanvien, value); }
        }

        private DateTime _Ngay;
        [XafDisplayName("Ngày"), ModelDefault("AllowEdit", "false")]
        [ModelDefault("EditMask", "dd/MM/yyyy")]
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Ngay
        {
            get { return _Ngay; }
            set { SetPropertyValue<DateTime>(nameof(Ngay), ref _Ngay, value); }
        }

        private string _Tailieu;
        [XafDisplayName("Tên tài liệu"), Size(255)]
        public string Tailieu
        {
            get { return _Tailieu; }
            set { SetPropertyValue<string>(nameof(Tailieu), ref _Tailieu, value); }
        }

        private string _TepTen;
        [XafDisplayName("Tên tệp"), Size(255)]
        public string TepTen
        {
            get { return _TepTen; }
            set { SetPropertyValue<string>(nameof(TepTen), ref _TepTen, value); }
        }

        private string _TepUrl;
        [XafDisplayName("URL"), Size(255)]
        public string TepUrl
        {
            get { return _TepUrl; }
            set { SetPropertyValue<string>(nameof(TepUrl), ref _TepUrl, value); }
        }

        //private FileData _Tep;
        //[XafDisplayName("Tệp")]
        //public FileData Tep
        //{
        //    get { return _Tep; }
        //    set { SetPropertyValue<FileData>(nameof(Tep), ref _Tep, value); }
        //}

        //1 - dx da duyet, 2 - nhat ky, 0 - tep dinh kem thong thuong
        private int _Loai;
        [Browsable(false)]
        public int Loai
        {
            get { return _Loai; }
            set { SetPropertyValue<int>(nameof(Loai), ref _Loai, value); }
        }
    }
}