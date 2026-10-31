using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinhky.Module.BusinessObjects
{
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("Bộ phận công tác")]
    [ImageName("bophan"), DefaultProperty("Name")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.Top)]
    [DeferredDeletion(false)]
    public class Bophan : XPObject, ITreeNode
    {
        private string name;
        private Bophan parent;
        [RuleFromBoolProperty("PhongbanCircularReferences", DefaultContexts.Save, "Circular refrerence detected. To correct this error, set the Parent property to another value.", UsedProperties = nameof(Parent))]

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                Bophan currentObj = Parent;
                while (currentObj != null)
                {
                    if (currentObj == this)
                    {
                        return false;
                    }
                    currentObj = currentObj.Parent;
                }
                return true;
            }
        }
        public Bophan(Session session) : base(session) { }

        public Bophan(Session session, string name)
    : this(session)
        {
            this.name = name;
        }

        [Association("NhomSPParent-NhomSPChild")]
        [XafDisplayName("Cấp dưới")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Bophan> Children
        {
            get { return GetCollection<Bophan>(nameof(Children)); }
        }
        [Persistent, Association("NhomSPParent-NhomSPChild")]
        [XafDisplayName("Cấp trên"), VisibleInListView(false)]
        public Bophan Parent
        {
            get { return parent; }
            set { SetPropertyValue(nameof(Parent), ref parent, value); }
        }

        //private int _SoTT;
        //public int SoTT
        //{
        //    get { return _SoTT; }
        //    set { SetPropertyValue<int>(nameof(SoTT), ref _SoTT, value); }
        //}


        [XafDisplayName("Tên bộ phận")]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue<string>(nameof(Name), ref name, value); }
        }

        private bool _Nhan;
        [XafDisplayName("Nơi nhận")]
        public bool Nhan
        {
            get { return _Nhan; }
            set { SetPropertyValue<bool>(nameof(Nhan), ref _Nhan, value); }
        }

        private bool _Thuchien;
        [XafDisplayName("Thực hiện")]
        public bool Thuchien
        {
            get { return _Thuchien; }
            set { SetPropertyValue<bool>(nameof(Thuchien), ref _Thuchien, value); }
        }

        private string _Diachi;
        [XafDisplayName("Địa chỉ")]
        public string Diachi
        {
            get { return _Diachi; }
            set { SetPropertyValue<string>(nameof(Diachi), ref _Diachi, value); }
        }

        private string _Dienthoai;
        [XafDisplayName("Điện thoại")]
        public string Dienthoai
        {
            get { return _Dienthoai; }
            set { SetPropertyValue<string>(nameof(Dienthoai), ref _Dienthoai, value); }
        }

        [XafDisplayName("Nhân viên")]
        public int SoNV
        {
            get { return Nhansus.Count; }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Nhân sự")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Nhansu> Nhansus
        {
            get { return GetCollection<Nhansu>(nameof(Nhansus)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Phiếu đề xuất"), Browsable(false)]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<Phieudexuat> Phieudexuats
        {
            get { return GetCollection<Phieudexuat>(nameof(Phieudexuats)); }
        }

        [DevExpress.Xpo.Aggregated, Association]
        [XafDisplayName("Mẫu sử dụng")]
        //[RuleRequiredField(DefaultContexts.Delete, InvertResult = true, CustomMessageTemplate = "'{TargetPropertyName}' có dữ liệu. Không xóa được!")]
        public XPCollection<MauBP> MauBPs
        {
            get { return GetCollection<MauBP>(nameof(MauBPs)); }
        }
        IBindingList ITreeNode.Children
        {
            get { return Children as IBindingList; }
        }
        ITreeNode ITreeNode.Parent
        {
            get { return Parent as ITreeNode; }
        }

    }
}
