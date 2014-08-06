﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates use of the GeometryEngine.ConvexHull method to create a convex hull polygon for three or more points.
    /// A convex hull is the smallest polygon that completely encases a set (i.e. locus) of points.
    /// </summary>
    /// <title>Convex Hull</title>
	/// <category>Geometry</category>
	public partial class ConvexHull : Windows.UI.Xaml.Controls.Page
    {
        private GraphicsOverlay _inputGraphicsOverlay;
        private GraphicsOverlay _convexHullGraphicsOverlay;

        private Symbol _pointSymbol;
        private Symbol _polygonSymbol;

        /// <summary>Construct Convex Hull sample control</summary>
        public ConvexHull()
        {
            InitializeComponent();

			_inputGraphicsOverlay = MyMapView.GraphicsOverlays[0];
			_convexHullGraphicsOverlay = MyMapView.GraphicsOverlays[1];
            _pointSymbol = (Symbol)layoutGrid.Resources["PointSymbol"];
            _polygonSymbol = (Symbol)layoutGrid.Resources["ConvexHullSymbol"];

            var _ = DrawPointsAsync();
        }

        // Continuosly accepts new points from the user
        private async Task DrawPointsAsync()
        {
            try
            {
                await MyMapView.LayersLoadedAsync();

                while (MyMapView.Extent != null)
                {
                    var point = await MyMapView.Editor.RequestPointAsync();

                    // reset graphics layers if we've already created a convex hull polygon
                    if (_convexHullGraphicsOverlay.Graphics.Count > 0)
                    {
						_inputGraphicsOverlay.Graphics.Clear();
                        _convexHullGraphicsOverlay.Graphics.Clear();
                    }

					_inputGraphicsOverlay.Graphics.Add(new Graphic(point, _pointSymbol));

					if (_inputGraphicsOverlay.Graphics.Count > 2)
                        btnConvexHull.IsEnabled = true;
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error adding points: " + ex.Message, "Sample Error").ShowAsync();
            }
        }

        // Creates a convex hull polygon from the input point graphics
        private void ConvexHullButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
				var convexHull = GeometryEngine.ConvexHull(_inputGraphicsOverlay.Graphics.Select(g => g.Geometry));
                _convexHullGraphicsOverlay.Graphics.Add(new Graphic(convexHull, _polygonSymbol));

                btnConvexHull.IsEnabled = false;
            }
            catch (Exception ex)
            {
                var _ = new MessageDialog("Error calculating convex hull: " + ex.Message, "Sample Error").ShowAsync();
            }
        }
    }
}
