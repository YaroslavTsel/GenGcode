using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;

namespace Source
{
   public static class ElemsLib
   {
      public static void HighlightElements(ObjectId[] selectedObjects)
      {
         Document curdoc = Application.DocumentManager.MdiActiveDocument;
         var database = curdoc.Database;
         var ed = curdoc.Editor;
         using (DocumentLock docLock = curdoc.LockDocument())
         {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
              // ed.SetImpliedSelection(selectedObjects);

               Entity ent = acTrans.GetObject(selectedObjects[0], OpenMode.ForWrite) as Entity;
               ent.Highlight();

               acTrans.Commit();
            }
            ed.UpdateScreen();
         }
      }
   }
}
