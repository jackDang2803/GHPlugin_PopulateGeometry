using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PopulatingGeo
{
    public class AddNumbersComponent : GH_Component
    {                                                       
        /// <summary>
        /// Initializes a new instance of the AddNumbersComponent class.
        /// </summary>
        public AddNumbersComponent()
          : base("AddNumbersComponent", "AddNums",
              "Adds two numbers together",
              "QuocComponent", "Operators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("NumberA", "A", "First value to add", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("NumberB", "B", "Second value to add", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Result", "R", "Addition result", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) // DA is data object
        {
            // Define placeholder variables
            double a = 0;
            double b = 0;

            // Load values from inputs into those variables
            if (!DA.GetData(0, ref a)) return;
            if (!DA.GetData(1, ref b)) return;

            // The code that actually does the work
            double sum = a + b;

            //Outputs
            DA.SetData(0, sum);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ECE2EA2D-F0F5-49B9-B113-132EFAB943E5"); }
        }
    }
}