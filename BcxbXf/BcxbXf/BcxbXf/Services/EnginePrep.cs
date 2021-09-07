

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using BcxbDataAccess;
using BCX.BCXB;
using BcxbXf.Services;

using Newtonsoft.Json;
using SimEngine;
using System.Reflection;

namespace BcxbXf {


   class EnginePrep {


      public CGame mGame { get; set; }
      //public DataAccess dataAccess = new();


      public static void SetupSimEngine(CGame mGame) {

         /* --------------------------------------------------------
          * The whole prurpose of this class is to build a CSimEngine object,
          * populating it with model file(s) (json), the 
          * GTab table, and team data, and finally injecting the model and
          * team objects into mGame as mGame.mSim and mGame.t, respectively.
          * --------------------------------------------------------
          */

         // Step 1. Load the engine (the model plus GTab.txt)
         // -----------------------
         CSimEngine sim = new();
         sim.RaiseHandler += mGame.DoSimAction;

         string jsonString;
         StreamReader rdr;
         //jsonString = ResourceReader.ReadEmbeddedRecouce("TestBcxbLib.Resources.Model.model1.json");
         //CModelBldr.LoadModel(jsonString, sim);

         //jsonString = ResourceReader.ReadEmbeddedRecouce("TestBcxbLib.Resources.Model.model1.json");
         //CModelBldr.LoadModel(jsonString1, sim);

         using (rdr = Repository.GetTextFileOnDisk("Model.TREE5-Lisp.json")) {
            jsonString = rdr.ReadToEnd();
         }
         CModelBldr.LoadModel(jsonString, sim);

         using (rdr = Repository.GetTextFileOnDisk("Model.AL5-Lisp.json")) {
            jsonString = rdr.ReadToEnd();
         }
         CModelBldr.LoadModel(jsonString, sim);

         using (rdr = Repository.GetTextFileOnDisk("Model.GTAB5.txt")) {
            CModelBldr.LoadGTab(rdr, sim);
         }

         mGame.mSim = sim; // Here we 'inject' the dependancy into the CGame obj.


      }


   }


}