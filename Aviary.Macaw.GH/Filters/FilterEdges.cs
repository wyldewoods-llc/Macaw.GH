﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using Aviary.Macaw.Filters.Edges;

namespace Aviary.Macaw.GH.Filters
{
    public class FilterEdges : GH_Component
    {
        private enum FilterModes { Difference, Homogeneity, Kirsch, Robinson, Sobel, Canny}

        /// <summary>
        /// Initializes a new instance of the FilterEdges class.
        /// </summary>
        public FilterEdges()
          : base("Filter Edges", "Edges", "Apply edge detection filters to an image" + Environment.NewLine + "Note: Not all filter modes use the additional parameter inputs." + Environment.NewLine + "Built on the Accord Imaging Library" + Environment.NewLine + "http://accord-framework.net/", "Aviary 1", "Image")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Image", "I", "An Aviary Image or Bitmap", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Mode", "M", "Select filter type", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Value A", "A", "---", GH_ParamAccess.item, 1.0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Value B", "B", "---", GH_ParamAccess.item, 1);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Value C", "C", "---", GH_ParamAccess.item, 1);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Value D", "D", "---", GH_ParamAccess.item, 1);
            pManager[5].Optional = true;

            Param_Integer param = (Param_Integer)pManager[1];
            foreach (FilterModes value in Enum.GetValues(typeof(FilterModes)))
            {
                param.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Image", "I", "An Aviary Image with the filter added to it", GH_ParamAccess.item);
            pManager.AddGenericParameter("Filter", "F", "The specified Filter", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_Goo goo = null;
            Image image = new Image();
            if (!DA.GetData(0, ref goo)) return;
            if (!goo.TryGetImage(ref image)) return;

            int mode = 0;
            DA.GetData(1, ref mode);

            double numValA = 1.0;
            DA.GetData(2, ref numValA);

            int numValB = 1;
            DA.GetData(3, ref numValB);

            int numValC = 1;
            DA.GetData(4, ref numValC);

            int numValD = 1;
            DA.GetData(5, ref numValD);

            Filter filter = new Filter();

            int[] indices = new int[] { 2, 3, 4, 5 };

            switch ((FilterModes)mode)
            {
                case FilterModes.Canny:
                    SetParameter(2, "X", "Sigma", "Gaussian sigma");
                    SetParameter(3, "S", "Size", "Gaussian size");
                    SetParameter(4, "L", "Low Threshold", "High threshold value used for hysteresis");
                    SetParameter(5, "H", "High Threshold", "Low threshold value used for hysteresis");
                    filter = new Canny(numValA, numValB, numValC, numValD);
                    image.Filters.Add(new Canny());
                    break;
                case FilterModes.Difference:
                    ClearParameter(indices);
                    filter = new Difference();
                    image.Filters.Add(new Difference());
                    break;
                case FilterModes.Homogeneity:
                    ClearParameter(indices);
                    filter = new Homogeneity();
                    image.Filters.Add(new Homogeneity());
                    break;
                case FilterModes.Kirsch:
                    ClearParameter(indices);
                    filter = new Kirsch();
                    image.Filters.Add(new Kirsch());
                    break;
                case FilterModes.Robinson:
                    ClearParameter(indices);
                    filter = new Robinson();
                    image.Filters.Add(new Robinson());
                    break;
                case FilterModes.Sobel:
                    ClearParameter(indices);
                    filter = new Sobel();
                    image.Filters.Add(new Sobel());
                    break;
            }

            DA.SetData(0, image);
            DA.SetData(1, filter);
        }

        protected void ClearParameter(int[] indices)
        {
            foreach(int i in indices)
            {
                SetParameter(i);
            }
        }

        protected void SetParameter(int index, string nickname = "-", string name = "Not Used", string description = "Parameter not used by this filter")
        {
            Params.Input[index].NickName = nickname;
            Params.Input[index].Name = name;
            Params.Input[index].Description = description;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Edges1;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1c3c96c3-f6fa-41f6-81f7-caa5139f0167"); }
        }
    }
}