using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XFvoice
{
    public class ImageButton : Button
    {
        public ImageSource NorImage
        {
            get { return (ImageSource)GetValue(NorImageProperty); }
            set { SetValue(NorImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NorImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NorImageProperty =
            DependencyProperty.Register("NorImage", typeof(ImageSource), typeof(ImageButton));



        public ImageSource HorImage
        {
            get { return (ImageSource)GetValue(HorImageProperty); }
            set { SetValue(HorImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorImageProperty =
            DependencyProperty.Register("HorImage", typeof(ImageSource), typeof(ImageButton));



        public ImageSource DownImage
        {
            get { return (ImageSource)GetValue(DownImageProperty); }
            set { SetValue(DownImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DownImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownImageProperty =
            DependencyProperty.Register("DownImage", typeof(ImageSource), typeof(ImageButton));


    }

    public class ImageRadioButtonButton : RadioButton
    {


        public ImageSource NorImage
        {
            get { return (ImageSource)GetValue(NorImageProperty); }
            set { SetValue(NorImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NorImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NorImageProperty =
            DependencyProperty.Register("NorImage", typeof(ImageSource), typeof(ImageRadioButtonButton));

        public ImageSource HorImage
        {
            get { return (ImageSource)GetValue(HorImageProperty); }
            set { SetValue(HorImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorImageProperty =
            DependencyProperty.Register("HorImage", typeof(ImageSource), typeof(ImageRadioButtonButton));



        public ImageSource SelectedImage
        {
            get { return (ImageSource)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DownImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(ImageSource), typeof(ImageRadioButtonButton));

    }
}
