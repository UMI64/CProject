using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CProject
{
    [Activity(Label = "CProject", MainLauncher = true)]
    public class MainActivity : Activity
    {
        List<ValueTable> Values = new List<ValueTable> { };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button InsertButton = FindViewById<Button>(Resource.Id.insertbutton);
            Button FindButton = FindViewById<Button>(Resource.Id.findbutton);
            Button ModifyButton = FindViewById<Button>(Resource.Id.modifybutton);
            Button DeleteButton = FindViewById<Button>(Resource.Id.deletebutton);
            EditText InsertText = FindViewById<EditText>(Resource.Id.insertText);
            EditText FindText = FindViewById<EditText>(Resource.Id.findtext);
            EditText ModifyText1 = FindViewById<EditText>(Resource.Id.modifytext1);
            EditText ModifyText2 = FindViewById<EditText>(Resource.Id.modifytext2);
            EditText DeleteText = FindViewById<EditText>(Resource.Id.delatetext);
            ListView ListText = FindViewById<ListView>(Resource.Id.PinList);
            /*获取路径*/
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            /*创建数据表*/
            db.CreateTable<StockTable>();
            InsertButton.Click += (s, arg) =>
            {
                if (InsertText.Text.Length > 0)
                {
                    var data = new StockTable();
                    data.Symbol = InsertText.Text;
                    db.Insert(data);
                }
                Values.Clear();
                foreach (StockTable Count in db.Table<StockTable>().ToList())
                {
                    ValueTable value = new ValueTable { Id = Count.Id, Symbol = Count.Symbol };
                    Values.Add(value);
                }
                ListText.Adapter = new MyCustomeAdapter(this, Values);
            };
            FindButton.Click += (s, arg) =>
            {
                Values.Clear();
                foreach (StockTable Count in (from item in db.Table<StockTable>() where item.Symbol.StartsWith(FindText.Text) select item).ToList())
                {
                    ValueTable value = new ValueTable { Id = Count.Id, Symbol = Count.Symbol };
                    Values.Add(value);
                }
                ListText.Adapter = new MyCustomeAdapter(this, Values);
            };
            ModifyButton.Click += (s, arg) =>
            {
                if (ModifyText1.Text.Length>0 && ModifyText2.Text.Length > 0)
                {
                    var result = (from item in db.Table<StockTable>() where item.Symbol.StartsWith(ModifyText1.Text) select item).ToList();
                    if (result.Count > 0)
                    {
                        result.First().Symbol = ModifyText2.Text;
                        db.Update(result);
                    }
                }
                Values.Clear();
                foreach (StockTable Count in db.Table<StockTable>().ToList())
                {
                    ValueTable value = new ValueTable { Id = Count.Id, Symbol = Count.Symbol };
                    Values.Add(value);
                }
                ListText.Adapter = new MyCustomeAdapter(this, Values);
            };
            DeleteButton.Click += (s, arg) =>
            {
                if (DeleteText.Text.Length > 0)
                {
                    var result = (from item in db.Table<StockTable>() where item.Symbol.StartsWith(DeleteText.Text) select item).ToList();
                    if (result.Count >0 )
                    {
                        db.Delete<StockTable>(result.First().Id);
                    }
                }
                Values.Clear();
                foreach (StockTable Count in db.Table<StockTable>().ToList())
                {
                    ValueTable value = new ValueTable { Id = Count.Id, Symbol = Count.Symbol };
                    Values.Add(value);
                }
                ListText.Adapter = new MyCustomeAdapter(this, Values);
            };
            ListText.ItemClick += (s, arg) =>
            {
                var t = Values[arg.Position];
                Android.Widget.Toast.MakeText(this, t.Symbol, Android.Widget.ToastLength.Short).Show();
            };

        }
    }
    public class StockTable
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(8)]
        public string Symbol { get; set; }
    }
    public class ValueTable
    {
        public int Id;
        public string Symbol;
    }
    public class MyCustomeAdapter : BaseAdapter<ValueTable>
    {
        List<ValueTable> items;
        Activity activity;

        public MyCustomeAdapter(Activity context, List<ValueTable> values) : base()
        {
            activity = context;
            items = values;
        }

        public override ValueTable this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null) v = activity.LayoutInflater.Inflate(Resource.Layout.PinListView, parent, false);

            var IdText = v.FindViewById<TextView>(Resource.Id.idtext);
            var ValueText = v.FindViewById<TextView>(Resource.Id.valuetext);

            IdText.Text = items[position].Id.ToString();
            ValueText.Text = items[position].Symbol;

            return v;
        }
    }
}

