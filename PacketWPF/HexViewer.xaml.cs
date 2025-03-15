using PacketWPF.NewFolder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketWPF
{
    /// <summary>
    /// Interaction logic for HexViewer.xaml
    /// </summary>
    public partial class HexViewer : UserControl
    {
        int remove = 0;

        int total = 0;

        public HexViewer()
        {
            InitializeComponent();
        }

        public void add(GroupRefrence reffrence)
        {
            total += reffrence.Data.Count;
            //foreach (var item in reffrence.Data)
            for(int i = 0; i < reffrence.Data.Count;i++)
            {
                var item = reffrence.Data[i];
                int x = 0;
                int y = 0;

                // int i = 0;// test.Children.Count - remove;

                x = i / 16;
                y = i % 16;

                // test.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

                if (y == 0)
                {
                    test.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    var line = (new Label { Content = BitConverter.ToString(BitConverter.GetBytes(total).Reverse().ToArray()).Replace("-", "") });
                    test.Children.Add(line);
                    Grid.SetRow(line, test.RowDefinitions.Count-1);
                    Grid.SetColumn(line, 0);

                    //remove++;
                }

                Label label = new Label { Content = item.Display };
                test.Children.Add(label);

                label.MouseEnter += (s, e) => Enter(s, e, item);
                label.MouseLeave += (s, e) => Leave(s, e, item);
                label.MouseDown += (s, e) => Click(s, e, i);

                item.LabelRefrence = label;

                Grid.SetRow(label, test.RowDefinitions.Count - 1);
                Grid.SetColumn(label, y+1);

                if (item.Group != null)
                {
                    label.Background = item.Group.b;
                }
            }


            /*for (int i = 0; i < Data.Length; i++)
            {
                var x = i / 16;
                var y = i % 16;

                if (y == 0)
                {
                    test.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    var line = (new Label { Content = BitConverter.ToString(BitConverter.GetBytes(i).Reverse().ToArray()).Replace("-", "") });
                    test.Children.Add(line);
                    Grid.SetRow(line, x);
                    Grid.SetColumn(line, 0);
                }

                var l = (new Label { Content = Data[i].Display });
                Data[i].LabelRefrence = l;
                var d = Data[i];
                if (d.Group != null)
                {
                    l.Background = d.Group.b;
                }
                l.MouseEnter += (s, e) => Enter(s, e, d);
                l.MouseLeave += (s, e) => Leave(s, e, d);
                l.MouseDown += (s, e) => Click(s, e, i);
                test.Children.Add(l);
                Grid.SetRow(l, x);
                Grid.SetColumn(l, y + 1);
            }*/
        }

        private void Enter(object sender, MouseEventArgs e, DataRefrences r)
        {
            if (r.Group != null)
            {
                r.Group.Data.ForEach(x => x.LabelRefrence.Background = Brushes.Yellow);
                return;
            }

            var l = (Label)sender;
            l.Background = Brushes.Yellow;
        }

        private void Leave(object sender, MouseEventArgs e, DataRefrences r)
        {
            if (r.Group != null)
            {
                r.Group.Data.ForEach(x => x.LabelRefrence.Background = r.Group.b);
                return;
            }

            var l = (Label)sender;
            l.Background = test.Background;
        }
        private void Click(object sender, MouseEventArgs e, int i)
        {

        }

        public void Reset()
        {
            test.Children.Clear();
            test.RowDefinitions.Clear();
            remove = 0;
            total = 0;
        }
    }
}
