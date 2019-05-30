using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TPS_Validation
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ViewModel();
		}

		private void Button_Click_UpdateAlgorithms(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_CalcBeams(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_RunEvaluation(object sender, RoutedEventArgs e)
		{

		}

		private void ComboBox_ValidatePhotonSelection(object sender, RoutedEventArgs e)
		{
			//loop through photon plans in patients and display somewhere any plans that the algorithm isn't available for and will skip if you continue to use this selection
		}
	}
}
