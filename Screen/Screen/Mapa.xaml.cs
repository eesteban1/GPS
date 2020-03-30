using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Screen
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Mapa : ContentPage
	{
		public Mapa ()
		{
			InitializeComponent ();

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(19.24212, -90.237137), Distance.FromMeters(30)));
            
		}
	}
}