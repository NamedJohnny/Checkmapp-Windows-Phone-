using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.Model.Tables;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using CheckMapp.Model.DataService;
using System.Collections.ObjectModel;

namespace CheckMapp.Controls
{
    public partial class TimelineControl : UserControl
    {
        public Canvas canvas;
        public Rectangle mainRectangle;

        public static readonly DependencyProperty TripsProperty =
     DependencyProperty.Register("Trips", typeof(ObservableCollection<Trip>), typeof(TimelineControl), null);

        public TimelineControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Liste des évènements de l'utilisateur
        /// </summary>
        public ObservableCollection<Trip> Trips
        {
            get { return base.GetValue(TripsProperty) as ObservableCollection<Trip>; }
            set
            {
                base.SetValue(TripsProperty, value);
                AdjustTimeLine();
            }
        }


        /// <summary>
        /// Type de date (mois, année)
        /// </summary>
        public enum TypeDate
        {
            Mois,
            Annee,
        };

        /// <summary>
        /// Crée les bordure pour les années et les mois
        /// </summary>
        /// <param name="date"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Border CreateBorder(TypeDate date, string text)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.Black);
            border.BorderBrush = new SolidColorBrush(Colors.White);
            TextBlock textBlock = new TextBlock();
            textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.Text = text;
            border.Child = textBlock;
            border.Width = date == TypeDate.Annee ? 120 : 80;
            border.Height = date == TypeDate.Annee ? 60 : 40;
            border.BorderThickness = new Thickness(date == TypeDate.Annee ? 6 : 4);
            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            textBlock.FontSize = date == TypeDate.Annee ? 26 : 20;
            canvas.Children.Add(border);
            return border;
        }

        /// <summary>
        /// Crée le visuel de la ligne du temps selon la liste des events
        /// </summary>
        public void AdjustTimeLine()
        {
            if (Trips.Count == 0)
            {
                return;
            }
            canvas = new Canvas();
            canvas.Width = 450;
            canvas.Opacity = 0.9;

            mainRectangle = new Rectangle();
            mainRectangle.Width = 10;
            mainRectangle.Height = 0;
            mainRectangle.Fill = new SolidColorBrush(Colors.White);

            canvas.Children.Add(mainRectangle);

            Canvas.SetLeft(mainRectangle, (canvas.Width / 2) - mainRectangle.Width / 2);

            //Obtient la liste des années
            var yearList = Trips.Select(item => new List<string>() { item.BeginDate.Year.ToString(), item.EndDate.Value.Year.ToString() })
                .SelectMany(group => group).Distinct();
            bool left = true;
            double previousBorderTop = 0;

            foreach (string year in yearList.OrderByDescending(x => x))
            {
                Border borderYear = CreateBorder(TypeDate.Annee, year);
                Canvas.SetTop(borderYear, previousBorderTop == 0 ? 0 : previousBorderTop + 250);
                Canvas.SetLeft(borderYear, (canvas.Width / 2) - borderYear.Width / 2);
                previousBorderTop = Canvas.GetTop(borderYear);

                bool firstMonth = true;

                //On obtient la liste des mois selon l'année
                var monthList = Trips.Where(x => x.BeginDate.Year.ToString() == year)
                    .Select(item => new List<int>() { item.BeginDate.Month/*, item.EndDate.Month*/ })
                    .SelectMany(group => group).Distinct().OrderBy(x => x);

                foreach (int month in monthList)
                {
                    string monthStr = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    Border borderMonth = CreateBorder(TypeDate.Mois, monthStr.Substring(0, 3));
                    Canvas.SetLeft(borderMonth, (canvas.Width / 2) - borderMonth.Width / 2);
                    //Si c'est le premier mois, alors plus près du border année
                    if (firstMonth)
                        Canvas.SetTop(borderMonth, Canvas.GetTop(borderYear) + borderYear.Height + 10);
                    else
                        Canvas.SetTop(borderMonth, previousBorderTop + 250);

                    firstMonth = false;
                    previousBorderTop = Canvas.GetTop(borderMonth);

                    foreach (Trip activites in Trips.Where(x => x.BeginDate.Year.ToString() == year && x.BeginDate.Month == month))
                    {
                        //Crée le cercle indiquant la position dans le mois¸¸
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = new SolidColorBrush(Colors.White);
                        ellipse.Height = 20;
                        ellipse.Width = 20;
                        int numMonth = activites.BeginDate.Month;
                        Canvas.SetTop(ellipse, Canvas.GetTop(borderMonth) + borderMonth.Height + (activites.BeginDate.Day * 3));
                        Canvas.SetLeft(ellipse, (canvas.Width / 2) - ellipse.Width / 2);
                        canvas.Children.Add(ellipse);

                        //Le contrôle affichant les détails d'un event
                        TimelineElementControl control = new TimelineElementControl(null, left);
                        left = !left;
                        control.Tap += control_Tap;
                        control.Trip = activites;
                        Canvas.SetTop(control, Canvas.GetTop(ellipse) + ellipse.Height / 4);
                        if (left)
                            Canvas.SetLeft(control, (canvas.Width / 2) - control.Width - 20);
                        else
                            Canvas.SetLeft(control, (canvas.Width / 2) + 30);

                        canvas.Children.Add(control);

                    }

                }
            }

            mainRectangle.Height = previousBorderTop + 200;
            canvas.Height = mainRectangle.Height + 100;
            LayoutRoot.Children.Add(canvas);
        }



        public event EventHandler UserControlElementTap;

        private void OnUserControlElementTap(Trip trip)
        {
            if (UserControlElementTap != null)
            {
                UserControlElementTap(trip, EventArgs.Empty);
            }
        }

        public void control_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            OnUserControlElementTap((sender as TimelineElementControl).Trip);
        }




    }
}
