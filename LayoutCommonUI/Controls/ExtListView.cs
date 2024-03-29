using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls {
    #region DataListView Class
    [
    Serializable(), System.ComponentModel.DesignerCategory("Code"),
    ToolboxBitmapAttribute(typeof(LayoutManager.CommonUI.Controls.DataListView), "DataListView.ico")
    ]
    public class DataListView : ListView {
        #region Variables used within Scope of this Class.
        private object? mDataSource = null;
        private string mDataMember = string.Empty;
        private string mNoDataMessage = "There are no data available at present.";
        private bool mAutoDiscovery = true;
        private static bool bDisposing = false;
        private bool mGridLines = false;
        private bool mUseItemStyleForSubItems = false;
        private const int WM_ERASEBKGND = 0x14;
        private readonly SolidBrush mSbBackColor = new(System.Drawing.Color.FromKnownColor(KnownColor.Window));
        private readonly SolidBrush mSbForeColor = new(System.Drawing.Color.FromKnownColor(KnownColor.WindowText));
        private IBindingList? mBindingList = null;
        private DataColumnHeaderCollection? mColumns = null;
        private Thread? t_Serialize = null;
        private Thread? t_DeSerialize = null;
        private Thread? t_ResizeColumns = null;
        private Thread? t_DataBind = null;
        private bool mThreadDataBind = false;
        private bool mThreadSerialization = false;
        private bool mThreadDeSerialization = false;
        private bool mThreadResizeColumns = true;
        private string mFilename = string.Empty;
        private bool mOverwrite = false;
        #endregion

        #region DataListView Constructor
        public DataListView() {
            this.View = System.Windows.Forms.View.Details;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.mColumns = new DataColumnHeaderCollection();
            //this.mColumns.Invalidate += new nsListViewEx.DataColumnHeaderCollection.InvalidateEventHandler(mColumns_Invalidate);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColorChanged += DataListView_BackColorChanged;
            this.ForeColorChanged += DataListView_ForeColorChanged;
        }
        #endregion

        #region Dispose Override - Protected
        protected override void Dispose(bool disposing) {
            if (disposing) {
                bDisposing = true;
                if (this.mSbBackColor != null) {
                    this.mSbBackColor.Dispose();
                }
                if (this.mSbForeColor != null) {
                    this.mSbForeColor.Dispose();
                }
                if (base.Columns.Count >= 1) {
                    base.Columns.Clear();
                }
                if (this.Items.Count >= 1) {
                    this.Items.Clear();
                }
                if (this.mBindingList != null) {
                    this.mBindingList.ListChanged -= MBindingList_ListChanged;
                    this.mBindingList = null;
                }
                if (this.mColumns != null) {
                    //this.mColumns.Invalidate -= new nsListViewEx.DataColumnHeaderCollection.InvalidateEventHandler(mColumns_Invalidate);
                    this.mColumns.Clear();
                    this.mColumns = null;
                }
                if (this.mDataSource != null) {
                    this.mDataSource = null;
                }
                //this.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region WndProc Override - Protected
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if (m.Msg == WM_ERASEBKGND) {
                #region Handle drawing of "no items" message
                if (Items.Count == 0 && Columns.Count == 0) {
                    if (this.mGridLines) {
                        base.GridLines = false;
                    }
                    using Graphics g = this.CreateGraphics();
                    using StringFormat sf = new() {
                        Alignment = StringAlignment.Center
                    };
                    int w = (this.Width - g.MeasureString(this.mNoDataMessage, this.Font).ToSize().Width) / 2;
                    Rectangle rc = new(0, (int)(this.Font.Height * 1.5), w, this.Height);
                    //g.FillRectangle(SystemBrushes.Window, 0, 0, this.Width, this.Height);
                    g.FillRectangle(this.mSbBackColor, 0, 0, this.Width, this.Height);
                    //g.DrawString(this.mNoDataMessage, this.Font, SystemBrushes.ControlText, w, 30);
                    g.DrawString(this.mNoDataMessage, this.Font, this.mSbForeColor, w, 30);
                }
                else {
                    base.GridLines = this.mGridLines;
                }
                #endregion
            }
        }
        #endregion

        #region DesignTime Get/Set Accessors
        #region DataSource Property
        [System.ComponentModel.Category("Data"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.RefreshProperties(RefreshProperties.Repaint),
        System.ComponentModel.TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
        System.ComponentModel.Description("Data Source.")]
        public object? DataSource {
            get { return this.mDataSource; }
            set {
                if (value != null) {
                    this.mDataSource = value;
                    this.SetSource();
                    if (this.mAutoDiscovery) {
                        this.DoAutoDiscovery();
                        if (this.Items.Count == 0) this.Invalidate();
                    }
                    this.DataBind();
                }
            }
        }
        #endregion

        #region DataMember Property
        [System.ComponentModel.Category("Data"),
        System.ComponentModel.Editor("System.Windows.Forms.Design.DataMemberListEditor,System.Design", typeof(System.Drawing.Design.UITypeEditor)),
        System.ComponentModel.Description("Data Member.")]
        public string DataMember {
            get { return this.mDataMember; }
            set {
                if (value.Length != 0) {
                    this.mDataMember = value;
                    //this.SetSource();
                    //if (this.mAutoDiscovery){
                    //	 this.DoAutoDiscovery();
                    //	 if (this.Items.Count == 0) this.Invalidate();
                    //}
                }
            }
        }
        #endregion

        #region AutoDiscovery Property
        [System.ComponentModel.Category("Data"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("AutoDiscovery.")]
        public bool AutoDiscovery {
            get { return this.mAutoDiscovery; }
            set {
                if (!this.mAutoDiscovery && value) {
                    this.mAutoDiscovery = value;
                    this.DoAutoDiscovery();
                    if (this.Items.Count == 0) this.Invalidate();
                }
                else {
                    this.mAutoDiscovery = value;
                }
            }
        }
        #endregion

        #region Columns Property
        [System.ComponentModel.Category("Data"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("Columns.")]
        public new DataColumnHeaderCollection? Columns => this.mColumns;
        #endregion

        #region GridLines Property
        [DefaultValue(false)]
        public new bool GridLines {
            get { return this.mGridLines; }
            set { this.mGridLines = value; Invalidate(); }
        }
        #endregion

        #region UnavailableDataMessage Property
        [System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("A default message to show when there is no data bound to this DataListView."),
        DefaultValue("There are no data available at present.")]
        public string UnavailableDataMessage {
            get { return this.mNoDataMessage; }
            set {
                if (!value.Equals(this.mNoDataMessage)) {
                    this.mNoDataMessage = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region UseItemStyleForSubItems Property
        [System.ComponentModel.Category("Appearance"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("A way of customizing each column style as per ListViewItem.UseItemStyleForSubItems."),
        DefaultValue(false)]
        public bool UseItemStyleForSubItems {
            get { return this.mUseItemStyleForSubItems; }
            set {
                if (this.mUseItemStyleForSubItems != value) {
                    this.mUseItemStyleForSubItems = value;
                }
            }
        }
        #endregion

        #region ThreadDataBind
        [System.ComponentModel.Category("Threading"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("Turn on Threading Support when data-binding to a large dataset or IList."),
        DefaultValue(false)]
        public bool DataBindThreading {
            get { return this.mThreadDataBind; }
            set {
                if (this.mThreadDataBind != value) {
                    this.mThreadDataBind = value;
                }
            }
        }
        #endregion

        #region ThreadSerialization
        [System.ComponentModel.Category("Threading"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("Turn on Threading Support when Serializing to Disk."),
        DefaultValue(false)]
        public bool SerializationThreading {
            get { return this.mThreadSerialization; }
            set {
                if (this.mThreadSerialization != value) {
                    this.mThreadSerialization = value;
                }
            }
        }
        #endregion

        #region ThreadDeSerialization
        [System.ComponentModel.Category("Threading"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("Turn on Threading Support when DeSerializing from Disk."),
        DefaultValue(false)]
        public bool DeSerializationThreading {
            get { return this.mThreadDeSerialization; }
            set {
                if (this.mThreadDeSerialization != value) {
                    this.mThreadDeSerialization = value;
                }
            }
        }
        #endregion

        #region ThreadReSizeColumns
        [System.ComponentModel.Category("Threading"),
        System.ComponentModel.Browsable(true),
        System.ComponentModel.Description("Turn on Threading Support when resizing columns."),
        DefaultValue(true)]
        public bool ResizeColumnsThreading {
            get { return this.mThreadResizeColumns; }
            set {
                if (this.mThreadResizeColumns != value) {
                    this.mThreadResizeColumns = value;
                }
            }
        }
        #endregion

        #region FullRowSelect Property - Turn this off
        [System.ComponentModel.Browsable(false)]
        public new bool FullRowSelect {
            get {
                base.FullRowSelect = true;
                return true;
            }
            set { base.FullRowSelect = true; }
        }
        #endregion

        #region View Property - Turn this off

        [System.ComponentModel.Browsable(false)]
        public new System.Windows.Forms.View View {
            get {
                base.View = System.Windows.Forms.View.Details;
                return System.Windows.Forms.View.Details;
            }
            set { base.View = System.Windows.Forms.View.Details; }
        }
        #endregion

        #region MultiSelect Property - Turn this off
        [System.ComponentModel.Browsable(false)]
        public new bool MultiSelect {
            get {
                base.MultiSelect = false;
                return false;
            }
            set { base.MultiSelect = false; }
        }
        #endregion

        #region LargeImageList Property - Turn this off
        [System.ComponentModel.Browsable(false)]
        public new ImageList? LargeImageList => null;
        #endregion

        #region SmallImageList Property - Turn this off
        [System.ComponentModel.Browsable(false)]
        public new ImageList? SmallImageList => null;
        #endregion

        #region StateImageList Property - Turn this off
        [System.ComponentModel.Browsable(false)]
        public new ImageList? StateImageList => null;
        #endregion
        #endregion

        #region DataBind Method - Private
        private void DataBind() {
            if (this.mThreadDataBind) {
                this.t_DataBind = new Thread(new ThreadStart(DataBindThread)) {
                    IsBackground = true,
                    Name = "Data Binding Thread"
                };
                this.t_DataBind.Start();
            }
            else {
                this.DataBinding();
            }
        }
        #endregion

        #region DataBindThread Method
        private delegate void DataBindDlgt();

        private void DataBindThread() {
            lock (this) {
                if (this.InvokeRequired) {
                    this.BeginInvoke(new DataBindDlgt(DataBinding));
                }
                else {
                    this.DataBinding();
                }
            }
        }

        private void DataBinding() {
            if (bDisposing) return;
            base.Clear();
            if (this.mDataSource == null) return;
            if (this.mColumns == null || this.mColumns.Count == 0) return;
            IList InnerSource = InnerDataSource();
            ListViewItem? lvi = null;
            Cursor current = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            this.BeginUpdate();
            for (int Field = 0; Field < this.mColumns.Count; Field++) {
                base.Columns.Add(this.mColumns[Field]);
            }
            for (int Row = 0; Row < InnerSource.Count; Row++) {
                lvi = new ListViewItem {
                    UseItemStyleForSubItems = this.mUseItemStyleForSubItems,
                    Text = this.GetField(InnerSource[Row], this.mColumns[0].Field) ?? String.Empty
                };
                for (int Field = 1; Field < this.mColumns.Count; Field++) {
                    lvi.SubItems.Add(this.GetField(InnerSource[Row], this.mColumns[Field].Field)).ToString();
                }
                this.Items.Add(lvi);
            }
            this.EndUpdate();
            this.Cursor = current;
        }
        #endregion

        #region InnerDataSource Function - Private
        private IList? InnerDataSource() {
            if (this.mDataSource is DataSet set) {
                return this.mDataMember.Length > 0
                    ? ((IListSource)set.Tables[this.mDataMember]).GetList()
                    : ((IListSource)set.Tables[0]).GetList();
            }
            else {
                return this.mDataSource is IListSource source ? source.GetList() : (IList?)this.mDataSource;
            }
        }
        #endregion

        #region SetSource Method - Private
        private void SetSource() {
            IList InnerSource = this.InnerDataSource();
            if (InnerSource is IBindingList) {
                this.mBindingList = (IBindingList)InnerSource;
                this.mBindingList.ListChanged += MBindingList_ListChanged;
            }
            else {
                this.mBindingList = null;
            }
        }
        #endregion

        #region mBindingList_ListChanged Event Handler
        private void MBindingList_ListChanged(object? sender, ListChangedEventArgs e) {
            this.DataBind();
            if (this.Items.Count == 0) this.Invalidate();
        }
        #endregion

        #region mColumns_Invalidate Event Handler
        private void mColumns_Invalidate() {
            this.DataBind();
            if (this.Items.Count == 0) this.Invalidate();
        }
        #endregion

        #region DoAutoDiscovery Overloads
        #region DoAutoDiscovery Method #1 - Private
        private void DoAutoDiscovery() {
            //if (this.mDataMember.Length == 0) return;
            if (this.mDataSource == null) return;
            IList InnerSource = InnerDataSource();
            this.mColumns.Clear();
            if (InnerSource == null) return;
            this.BeginUpdate();
            if (InnerSource is DataView) {
                DoAutoDiscovery((DataView)InnerSource);
            }
            else {
                DoAutoDiscovery(InnerSource);
            }
            this.EndUpdate();
        }
        #endregion

        #region DoAutoDiscovery Method #2 - Private
        private void DoAutoDiscovery(DataView ds) {
            int Field;
            DataColumnHeader Col;
            for (Field = 0; Field < ds.Table.Columns.Count; Field++) {
                if (ds.Table.Columns[Field].ColumnMapping != MappingType.Hidden) {
                    Col = new DataColumnHeader {
                        Text = ds.Table.Columns[Field].Caption,
                        Field = ds.Table.Columns[Field].ColumnName
                    };
                    this.mColumns.Add(Col);
                }
            }
        }
        #endregion

        #region DoAutoDiscovery Method #3 - Private
        private void DoAutoDiscovery(IList ds) {
            if (ds.Count > 0) {
                object obj = ds[0];
                if (obj is ValueType && obj.GetType().IsPrimitive) {
                    DataColumnHeader Col = new() {
                        Text = "Value"
                    };
                    this.mColumns.Add(Col);
                }
                else {
                    if (obj is string) {
                        DataColumnHeader Col = new() {
                            Text = "String"
                        };
                        this.mColumns.Add(Col);
                    }
                    else {
                        Type SourceType = obj.GetType();
                        PropertyInfo[] props = SourceType.GetProperties();
                        if (props.Length >= 0) {
                            for (int column = 0; column < props.Length; column++) {
                                this.mColumns.Add(props[column].Name);
                            }
                        }
                        FieldInfo[] fields = SourceType.GetFields();
                        if (fields.Length >= 0) {
                            for (int column = 0; column < fields.Length; column++) {
                                this.mColumns.Add(fields[column].Name);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region GetField Function - Private
        private string? GetField(object? obj, string FieldName) {
            if (obj == null)
                return "(null)";

            if (obj is DataRowView view) {
                return view[FieldName].ToString();
            }
            else {
                if (obj is ValueType && obj.GetType().IsPrimitive) {
                    return obj.ToString();
                }
                else {
                    if (obj is string) {
                        return (string)obj;
                    }
                    else {
                        try {
                            Type SourceType = obj.GetType();
                            var prop = obj.GetType().GetProperty(FieldName);
                            if (prop == null || !prop.CanRead) {
                                var field = SourceType.GetField(FieldName);
                                return field == null ? "(null)" : field.GetValue(obj)?.ToString();
                            }
                            else {
                                return prop.GetValue(obj, null)?.ToString();
                            }
                        }
                        catch (Exception) {
                            return "(null)";
                        }
                    }
                }
            }
        }
        #endregion

        #region RebindDataListView Method - Public
        public void RebindDataListView() {
            this.DataBind();
        }
        #endregion

        #region Clear Method - Public
        public new void Clear() {
            base.Clear();
            this.Items.Clear();
            this.Columns.Clear();
        }
        #endregion

        #region DataListView_BackColorChanged Event Handler
        private void DataListView_BackColorChanged(object? sender, EventArgs e) {
            this.mSbBackColor.Color = this.BackColor;
        }
        #endregion

        #region DataListView_ForeColorChanged Event Handler
        private void DataListView_ForeColorChanged(object? sender, EventArgs e) {
            this.mSbForeColor.Color = this.ForeColor;
        }
        #endregion

        #region GetLargestTextExtent Method - Private
        private void GetLargestTextExtent(DataListView dlv, int colNumber, ref int largestWidth) {
            int maxLen = -1;
            ListViewItem lvi = null;
            if (this.Items.Count >= 1) {
                if (colNumber >= 0 && colNumber < this.mColumns.Count) {
                    using Graphics g = this.CreateGraphics();
                    int newWidth = -1;
                    for (int nLoopCnt = 0; nLoopCnt < this.Items.Count; nLoopCnt++) {
                        lvi = (ListViewItem)this.Items[nLoopCnt] as ListViewItem;
                        if (lvi != null) {
                            newWidth = (int)g.MeasureString(lvi.SubItems[colNumber].Text, this.Font).Width;
                        }
                        else {
                            newWidth = 0;
                        }
                        if (newWidth > maxLen) {
                            maxLen = newWidth;
                        }
                    }
                    g.Dispose();
                }
            }
            largestWidth = maxLen;
        }
        #endregion

        #region GetLargestColHdrTextExtent Method - Private
        private void GetLargestColHdrTextExtent(DataListView dlv, int colNumber, ref int largestWidth) {
            if (this.Items.Count >= 1) {
                if (colNumber >= 0 && colNumber < this.mColumns.Count) {
                    using Graphics g = this.CreateGraphics();
                    largestWidth = (int)g.MeasureString(this.mColumns[colNumber].Text, this.Font).Width;
                    g.Dispose();
                }
            }
        }
        #endregion

        #region ResizeColumns Method - Public
        public void ResizeColumns() {
            if (this.mThreadResizeColumns) {
                this.t_ResizeColumns = new Thread(new ThreadStart(ResizeColumnsThread)) {
                    IsBackground = true,
                    Name = "Resize Columns Thread"
                };
                this.t_ResizeColumns.Start();
            }
            else {
                this.ResizeCols();
            }
        }
        #endregion

        #region ResizeColumnsThread Method - Public
        private delegate void ResizeColumnsDlgt();

        private void ResizeColumnsThread() {
            lock (this) {
                if (this.InvokeRequired) {
                    this.BeginInvoke(new ResizeColumnsDlgt(ResizeCols));
                }
                else {
                    this.ResizeCols();
                }
            }
        }

        private void ResizeCols() {
            Cursor current = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            if (this.Items.Count >= 1) {
                if (this.mColumns.Count >= 1) {
                    this.BeginUpdate();
                    for (int nLoopCnt = 0; nLoopCnt < this.mColumns.Count; nLoopCnt++) {
                        int nColHdrSize = 0, nColSize = 0;
                        this.GetLargestColHdrTextExtent(this, nLoopCnt, ref nColHdrSize);
                        this.GetLargestTextExtent(this, nLoopCnt, ref nColSize);
                        if (nColHdrSize > nColSize) {
                            this.mColumns[nLoopCnt].Width = nColHdrSize + 8; // Fudge Factor
                        }
                        else {
                            this.mColumns[nLoopCnt].Width = nColSize + 8;
                        }
                        nColHdrSize = nColSize = 0;
                    }
                    this.EndUpdate();
                }
            }
            this.Cursor = current;
        }
        #endregion

        #region SerializeToDisk Function
        public void SerializeToDisk(string FileName, bool Overwrite) {
            if (this.mThreadSerialization) {
                this.mOverwrite = Overwrite;
                this.mFilename = FileName;
                this.t_Serialize = new Thread(new ThreadStart(SerializeToDiskThread)) {
                    Name = "Serializing to Disk Thread",
                    IsBackground = true
                };
                this.t_Serialize.Start();
            }
            else {
                this.Serialize2Disk(FileName, Overwrite);
            }
        }
        #endregion

        #region SerializeToDisk Threading
        private delegate void SerializeToDiskDlgt(string FileName, bool Overwrite);

        private void SerializeToDiskThread() {
            lock (this) {
                if (this.InvokeRequired) {
                    this.BeginInvoke(new SerializeToDiskDlgt(Serialize2Disk), new object[] { this.mFilename, this.mOverwrite });
                }
                else {
                    this.Serialize2Disk(this.mFilename, this.mOverwrite);
                }
            }
        }

        private void Serialize2Disk(string FileName, bool Overwrite) {
            int nItemsCount = this.Items.Count;
            FileStream fs = null;
            bool bFileError = false;
            Cursor current = Cursor.Current;
            this.Cursor = Cursors.WaitCursor;
            if (nItemsCount >= 1) {
                DataLstView dlvItems = new() {
                    DataListViewItems = new ListViewItem[nItemsCount],
                    DataListViewTags = new object[nItemsCount]
                };
                this.Items.CopyTo(dlvItems.DataListViewItems, 0);
                dlvItems.ColumnNames = new string[this.Columns.Count];
                dlvItems.ColumnAlignment = new byte[this.Columns.Count];
                dlvItems.ColumnWidth = new int[this.Columns.Count];
                for (int nLoopCnt = 0; nLoopCnt < this.Columns.Count; nLoopCnt++) {
                    dlvItems.ColumnNames[nLoopCnt] = this.Columns[nLoopCnt].Text;
                    dlvItems.ColumnAlignment[nLoopCnt] = (byte)this.Columns[nLoopCnt].TextAlign;
                    dlvItems.ColumnWidth[nLoopCnt] = (int)this.Columns[nLoopCnt].Width;
                }
                for (int nLoopCnt = 0; nLoopCnt < nItemsCount; nLoopCnt++) {
                    ListViewItem lvi = (ListViewItem)this.Items[nLoopCnt];
                    dlvItems.DataListViewTags[nLoopCnt] = lvi.Tag;
                }
                try {
                    if (Overwrite) {
                        fs = new FileStream(FileName, FileMode.OpenOrCreate);
                    }
                    else {
                        fs = new FileStream(FileName, FileMode.Create);
                    }
                }
                catch (ArgumentException) {
                    bFileError = true;
                    throw;
                }
                catch (FileNotFoundException) {
                    bFileError = true;
                    throw;
                }
                catch (IOException) {
                    bFileError = true;
                    throw;
                }
                if (!bFileError) {
                    try {
                        BinaryFormatter bf = new();
                        bf.Serialize(fs, dlvItems);
                    }
                    catch (SerializationException) {
                        throw;
                    }
                    catch (Exception) {
                        throw;
                    }
                    finally {
                        fs.Close();
                    }
                }
                this.Cursor = current;
            }
            this.Cursor = current;
        }
        #endregion

        #region DeSerializeFromDisk Function
        public void DeSerializeFromDisk(string FileName) {
            if (this.mThreadDeSerialization) {
                this.mFilename = FileName;
                this.t_DeSerialize = new Thread(new ThreadStart(DeSerializeFromDiskThread)) {
                    Name = "Deserializing from Disk Thread",
                    IsBackground = true
                };
                this.t_DeSerialize.Start();
            }
            else {
                this.DeSerializeFrmDisk(FileName);
            }
        }
        #endregion

        #region DeSerializeFromDisk Threading
        private delegate void DeSerializeFromDiskDlgt(string FileName);

        private void DeSerializeFromDiskThread() {
            lock (this) {
                if (this.InvokeRequired) {
                    this.BeginInvoke(new DeSerializeFromDiskDlgt(DeSerializeFrmDisk), new object[] { this.mFilename });
                }
                else {
                    this.DeSerializeFrmDisk(this.mFilename);
                }
            }
        }

        private void DeSerializeFrmDisk(string FileName) {
            FileStream fs = null;
            DataLstView dlvItems = new();
            bool bFileError = false;
            Cursor current = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try {
                fs = new FileStream(FileName, FileMode.Open);
            }
            catch (ArgumentException) {
                bFileError = true;
                throw;
            }
            catch (FileNotFoundException) {
                bFileError = true;
                throw;
            }
            catch (IOException) {
                bFileError = true;
                throw;
            }
            if (!bFileError) {
                this.BeginUpdate();
                try {
                    BinaryFormatter bf = new();
                    this.DataSource = null;
                    base.Columns.Clear();
                    this.mColumns.Clear();
                    this.Items.Clear();
                    dlvItems = (DataLstView)bf.Deserialize(fs);
                    if (dlvItems.ColumnNames.Length >= 1) {
                        for (int nLoopCnt = 0; nLoopCnt < dlvItems.ColumnNames.Length; nLoopCnt++) {
                            base.Columns.Add(dlvItems.ColumnNames[nLoopCnt], dlvItems.ColumnWidth[nLoopCnt], (HorizontalAlignment)dlvItems.ColumnAlignment[nLoopCnt]);
                            //base.Columns.Add(dlvItems.ColumnNames[nLoopCnt], -1, (HorizontalAlignment)dlvItems.ColumnAlignment[nLoopCnt]);
                            this.mColumns.Add(dlvItems.ColumnNames[nLoopCnt], dlvItems.ColumnWidth[nLoopCnt]);
                        }
                    }
                    base.Items.AddRange(dlvItems.DataListViewItems);
                    if (dlvItems.DataListViewTags.Length >= 1) {
                        for (int nLoopCnt = 0; nLoopCnt < dlvItems.DataListViewTags.Length; nLoopCnt++) {
                            ListViewItem lvi = this.Items[nLoopCnt];
                            lvi.Tag = dlvItems.DataListViewTags[nLoopCnt];
                        }
                    }
                }
                catch (SerializationException) {
                    throw;
                }
                catch (Exception) {
                    throw;
                }
                finally {
                    fs.Close();
                    this.EndUpdate();
                }
            }
            this.Cursor = current;
            if (this.Items.Count >= 1) {
                this.ResizeColumns();
            }
        }
        #endregion
    }
    #endregion

    #region DataColumnHeader Class
    [Serializable()]
    public class DataColumnHeader : ColumnHeader {
        public string Field { get; set; }
    }
    #endregion

    #region DataColumnHeaderCollection Class
    [Serializable()]
    public class DataColumnHeaderCollection : CollectionBase {
        public delegate void InvalidateEventHandler();
        public event InvalidateEventHandler Invalidate;

        protected virtual void OnInvalidate() {
            this.Invalidate?.Invoke();
        }

        public DataColumnHeader this[int Index] {
            get {
                bool bError = false;
                try {
                    if (List.Count >= 1) {
                        return (DataColumnHeader)List[Index];
                    }
                    bError = true;
                }
                catch (System.ArgumentOutOfRangeException) {
                    bError = true;
                    throw new System.ArgumentOutOfRangeException(nameof(Index), (int)Index, "There is no such index value in this collection\nSource: ExtListView:nsListViewEx.DataColumnHeaderCollection");
                }
                return bError ? null : (DataColumnHeader)null;
            }
        }

        public void Add(string Field) {
            DataColumnHeader col = new() {
                Text = Field,
                Field = Field
            };
            List.Add((DataColumnHeader)col);
        }

        public void Add(string Field, int Width) {
            DataColumnHeader col = new() {
                Text = Field,
                Field = Field,
                Width = Width
            };
            List.Add((DataColumnHeader)col);
        }

        public void Add(string Text, string Field) {
            DataColumnHeader col = new() {
                Text = Text,
                Field = Field
            };
            List.Add((DataColumnHeader)col);
        }

        public void Add(string Text, string Field, int Width) {
            DataColumnHeader col = new() {
                Text = Text,
                Field = Field,
                Width = Width
            };
            List.Add((DataColumnHeader)col);
        }

        public void Add(DataColumnHeader Item) {
            List.Add((DataColumnHeader)Item);
        }

        protected override void OnRemoveComplete(int index, object value) {
            this.OnInvalidate();
        }

        protected override void OnInsertComplete(int index, object value) {
            this.OnInvalidate();
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue) {
            this.OnInvalidate();
        }

        protected override void OnClearComplete() {
            this.OnInvalidate();
        }
    }
    #endregion

    #region DataLstView Class
    [Serializable()]
    public class DataLstView {
        #region Private Variables within this scope
        #endregion

        #region DataLstView Constructor - Empty for Serialization
        public DataLstView() { }
        #endregion

        #region DataListViewItems Get/Set Accessor
        public ListViewItem[] DataListViewItems { get; set; }
        #endregion

        #region ColumnNames Get/Set Accessor
        public string[] ColumnNames { get; set; }
        #endregion

        #region ColumnAlignment Get/Set Accessor
        public byte[] ColumnAlignment { get; set; }
        #endregion

        #region ColumnWidth Get/Set Accessor
        public int[] ColumnWidth { get; set; }
        #endregion

        #region DataListViewTags Get/Set Accessor
        public object[] DataListViewTags { get; set; }
        #endregion
    }
    #endregion
}
