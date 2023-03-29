using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using Rhino;
using Rhino.Geometry;

namespace QuocGHPlugin.GeometryComps
{
    public class RanPopSurfComponent : GH_Component
    {
        ///██████╗  ██████╗ ██████╗ ██╗   ██╗██╗      █████╗ ████████╗██╗███╗   ██╗ ██████╗     
        ///██╔══██╗██╔═══██╗██╔══██╗██║   ██║██║     ██╔══██╗╚══██╔══╝██║████╗  ██║██╔════╝     
        ///██████╔╝██║   ██║██████╔╝██║   ██║██║     ███████║   ██║   ██║██╔██╗ ██║██║  ███╗    
        ///██╔═══╝ ██║   ██║██╔═══╝ ██║   ██║██║     ██╔══██║   ██║   ██║██║╚██╗██║██║   ██║    
        ///██║     ╚██████╔╝██║     ╚██████╔╝███████╗██║  ██║   ██║   ██║██║ ╚████║╚██████╔╝    
        ///╚═╝      ╚═════╝ ╚═╝      ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝     
        ///███████╗██╗   ██╗██████╗ ███████╗ █████╗  ██████╗███████╗                            
        ///██════╝██║   ██║██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝                            
        ///███████╗██║   ██║██████╔╝█████╗  ███████║██║     █████╗                              
        ///╚════██║██║   ██║██╔══██╗██╔══╝  ██╔══██║██║     ██╔══╝                              
        ///███████║╚██████╔╝██║  ██║██║     ██║  ██║╚██████╗███████╗                            
        ///╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝  ╚═╝ ╚═════╝╚══════╝  
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public RanPopSurfComponent()
          : base("Randomly Populating Geometry on Surface", "RndPopSur",
              "Randomly Populating Geometry on Surface",
              "QuocComponent", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Surface Height", "s", "Total Height of Surface", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Inner Radius", "R1", "Inner Radius of Surface", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Outter Radius", "R2", "Outter Radius of Surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("density", "density", "Amount of populated geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("spacing", "spacing", "Distance amongst populated geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("baseWidthG", "baseWidthG", "Base dimension of Cone geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("baseWidthB", "baseWidthB", "Base dimension of Box geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("heightFactorG", "heightFactorG", "Height of Cone geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("heightFactorB", "heightFactorB", "Height of Box geometry", GH_ParamAccess.item);
            pManager.AddIntegerParameter("seed", "seed", "control seed of random function", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "A", "Generated Surace", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "Points on Generated Surace", GH_ParamAccess.list);
            pManager.AddBoxParameter("Box", "B", "Populated Box", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Cone", "G", "Populated Box", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //double s, int R1, int R2, double density, double spacing, double baseWidthG, double baseWidthB, double heightFactorG, double heightFactorB, int seed,
            double s =0, density=0, spacing = 0, baseWidthG = 0, baseWidthB = 0, heightFactorG = 0, heightFactorB = 0;
            int R1 = 0, R2 = 0, seed = 0;

            if (!DA.GetData(0, ref s)) return;
            if (!DA.GetData(1, ref R1)) return;
            if (!DA.GetData(2, ref R2)) return;
            if (!DA.GetData(3, ref density)) return;
            if (!DA.GetData(4, ref spacing)) return;
            if (!DA.GetData(5, ref baseWidthG)) return;
            if (!DA.GetData(6, ref baseWidthB)) return;
            if (!DA.GetData(7, ref heightFactorG)) return;
            if (!DA.GetData(8, ref heightFactorB)) return;
            if (!DA.GetData(9, ref seed)) return;

            int smallR = Math.Min(R1, R2);
            int largeR = Math.Max(R1, R2);
            List<Point3d> pts = new List<Point3d>();
            List<double> listz = new List<double>();
            List<Box> buildings = new List<Box>();
            List<Cone> glaciers = new List<Cone>();
            //double waterLevel = s * 2 * 0.33 - s;
            //double cityLevel = s * 2 * 0.66 - s;

            for (int i = -360; i < 360; i++)
            {
                double rad = RhinoMath.ToRadians(i);
                double x, y, z;

                //latStart to define the orientation in XY plane of the box along with the curvature of the surface
                Point3d latStart = new Point3d((smallR + s * Math.Cos(0.5 * rad)) * Math.Cos(rad),
                  (smallR + s * Math.Cos(0.5 * rad)) * Math.Sin(rad), s * Math.Sin(0.5 * rad));

                for (int j = smallR; j < largeR; j++)
                {
                    x = (j + s * Math.Cos(0.5 * rad)) * Math.Cos(rad);
                    y = (j + s * Math.Cos(0.5 * rad)) * Math.Sin(rad);
                    z = s * Math.Sin(0.5 * rad);
                    Point3d pt = new Point3d(x, y, z);

                    pts.Add(pt);
                    System.Random rand = new System.Random(i * 10 + j + seed);

                    double rand_num = rand.NextDouble();

                    double sizeFactorG = rand.NextDouble();
                    //double randSizeG = (0.4 + (sizeFactorG * (1 - 0.4 + Math.Abs(z / s) / 2))) * baseWidthG;
                    //double randHeightG = (0.2 + (sizeFactorG * (1 - 0.2 + Math.Abs(z / s) / 2))) * heightFactorG;
                    double randSizeG = (0.2 + (sizeFactorG * 0.8)) * ((z + s) / s) * baseWidthG;
                    double randHeightG = (0.2 + (sizeFactorG * 0.8)) * ((z + s) / s) * heightFactorG;

                    double sizeFactorB = rand.NextDouble();
                    //double randSizeB = (0.6 + (sizeFactorB * (1 - 0.6 + Math.Abs(z / s)))) * baseWidthB;
                    //double randHeightB = (0.2 + (sizeFactorB * (1 - 0.2 + Math.Abs(z / s)))) * heightFactorB;
                    double randSizeB = (0.4 + (sizeFactorB * 0.2)) * 4 / (z + s + 1) * baseWidthB;
                    double randHeightB = (0.6 + (sizeFactorB * 0.2)) * 2 / (z + s + 1) * heightFactorB;

                    //Print(randSizeG.ToString());

                    //if (i % spacing == 0 && j % spacing == 0 && rand_num < density)
                    if (rand_num < density)
                    {
                        //randomly distibute between glacier or building in the middle, based on z

                        //Component written by @bava-kumaravel
                        //Creates a list of N random numbers ROut with the seed S
                        //within the domain of RIn

                        //Define a random object with seed S

                        //Get the minimum and maximum limits of the domain
                        double min = -1;
                        double max = 1;

                        double factor = rand.NextDouble();
                        //Use this as a factor to find a random number
                        //in the domain from min to max
                        double num = min + (factor * (max - min));

                        if (num >= (z / s))
                        {
                            Vector3d latVec = new Vector3d(pt - latStart);
                            Vector3d normal = new Vector3d(0, 0, 1);
                            Plane basePl = new Plane(pt, normal);
                            Interval intU = new Interval(-randSizeB / 10, randSizeB / 10);
                            Interval intV = new Interval(-randSizeB / 10, randSizeB / 10);

                            Interval intZ = new Interval(0, randHeightB);

                            Box building = new Box(basePl, intU, intV, intZ);
                            buildings.Add(building);
                        }
                        else
                        {
                            double glaOdd = rand.NextDouble();
                            if (glaOdd < 0.4)
                            {
                                if (z < -s * 0.6)
                                {
                                    randHeightG /= 2;
                                    randSizeG /= 2;
                                }
                                Vector3d latVec = new Vector3d(pt - latStart);
                                Vector3d normal = new Vector3d(0, 0, 1);
                                Plane basePl = new Plane(pt, -normal);
                                basePl.Translate(new Vector3d(0, 0, randHeightG));
                                Cone glacier = new Cone(basePl, randHeightG, randSizeG / 10);
                                glaciers.Add(glacier);
                            }
                        }
                    }
                }
            }
            //to connect the last set of points and the fist set of points, making a continuous surface
            //pts.Add(pts[1]);

            //****uCount multiplied by vCount must equal the number of points supplied for NurbsSurface.CreateThroughPoints****
            //create surface
            Surface srf = NurbsSurface.CreateThroughPoints(pts, 720, largeR - smallR, 3, 3, false, false);

            DA.SetData(0, srf);
            DA.SetDataList(1, pts);
            DA.SetDataList(2, buildings);
            DA.SetDataList(2, glaciers);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GH_plugin;

        //protected override System.Drawing.Bitmap Icon
        //{
        //    get
        //    {
        //        //You can add image files to your project resources and access them like this:
        //        // return Resources.IconForThisComponent;
        //        return Properties.Resources.GH_plugin;
        //    }
        //}

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7853E5F5-6315-40BE-B3D9-2E6F7DA5495B"); }
        }
    }
}