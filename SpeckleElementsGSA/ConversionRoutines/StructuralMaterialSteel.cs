﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpeckleCore;
using SpeckleCoreGeometryClasses;
using SpeckleElements;

namespace SpeckleElementsGSA
{
  [GSAObject("MAT_STEEL.3", new string[] { }, "properties", true, true, new Type[] { }, new Type[] { })]
  public class GSAMaterialSteel : IGSASpeckleContainer
  {
    public string GWACommand { get; set; }
    public List<string> SubGWACommand { get; set; } = new List<string>();
    public dynamic Value { get; set; } = new StructuralMaterialSteel();

    public void ParseGWACommand(GSAInterfacer GSA)
    {
      if (this.GWACommand == null)
        return;

      StructuralMaterialSteel obj = new StructuralMaterialSteel();

      string[] pieces = this.GWACommand.ListSplit(",");

      int counter = 1; // Skip identifier
      obj.StructuralId = pieces[counter++];
      counter++; // MAT.8
      obj.Name = pieces[counter++].Trim(new char[] { '"' });
      counter++; // Unlocked
      obj.YoungsModulus = Convert.ToDouble(pieces[counter++]);
      obj.PoissonsRatio = Convert.ToDouble(pieces[counter++]);
      obj.ShearModulus = Convert.ToDouble(pieces[counter++]);
      obj.Density = Convert.ToDouble(pieces[counter++]);
      obj.CoeffThermalExpansion = Convert.ToDouble(pieces[counter++]);

      // Failure strain is found before MAT_CURVE_PARAM.2
      int strainIndex = Array.FindIndex(pieces, x => x.StartsWith("MAT_CURVE_PARAM"));
      if (strainIndex > 0)
        obj.MaxStrain = Convert.ToDouble(pieces[strainIndex - 1]);

      // Skip to last fourth to last
      counter = pieces.Count() - 4;
      obj.YieldStrength = Convert.ToDouble(pieces[counter++]);
      obj.UltimateStrength = Convert.ToDouble(pieces[counter++]);

      this.Value = obj;
    }

    public void SetGWACommand(GSAInterfacer GSA)
    {
      if (this.Value == null)
        return;

      StructuralMaterialSteel mat = this.Value as StructuralMaterialSteel;

      string keyword = typeof(GSAMaterialSteel).GetGSAKeyword();

      int index = GSA.Indexer.ResolveIndex(typeof(GSAMaterialSteel), mat);

      // TODO: This function barely works.
      List<string> ls = new List<string>();

      ls.Add("SET");
      ls.Add("MAT_STEEL.3" + ":" + GSA.GenerateSID(mat));
      ls.Add(index.ToString());
      ls.Add("MAT.8");
      ls.Add(mat.Name == null || mat.Name == "" ? " " : mat.Name);
      ls.Add("YES"); // Unlocked
      ls.Add(mat.YoungsModulus.ToString()); // E
      ls.Add(mat.PoissonsRatio.ToString()); // nu
      ls.Add(mat.ShearModulus.ToString()); // G
      ls.Add(mat.Density.ToString()); // rho
      ls.Add(mat.CoeffThermalExpansion.ToString()); // alpha
      ls.Add("MAT_ANAL.1");
      ls.Add("0"); // TODO: What is this?
      ls.Add("Steel");
      ls.Add("-268435456"); // TODO: What is this?
      ls.Add("MAT_ELAS_ISO");
      ls.Add("6"); // TODO: What is this?
      ls.Add(mat.YoungsModulus.ToString()); // E
      ls.Add(mat.PoissonsRatio.ToString()); // nu
      ls.Add(mat.Density.ToString()); // rho
      ls.Add(mat.CoeffThermalExpansion.ToString()); // alpha
      ls.Add(mat.ShearModulus.ToString()); // G
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add("0"); // TODO: What is this?
      ls.Add(mat.MaxStrain.ToString()); // Ultimate strain
      ls.Add("MAT_CURVE_PARAM.2");
      ls.Add("");
      ls.Add("UNDEF");
      ls.Add("1"); // Material factor on strength
      ls.Add("1"); // Material factor on elastic modulus
      ls.Add("MAT_CURVE_PARAM.2");
      ls.Add("");
      ls.Add("UNDEF");
      ls.Add("1"); // Material factor on strength
      ls.Add("1"); // Material factor on elastic modulus
      ls.Add("0"); // Cost
      ls.Add(mat.YieldStrength.ToString()); // Yield strength
      ls.Add(mat.UltimateStrength.ToString()); // Ultimate strength
      ls.Add("0"); // Perfectly plastic strain limit
      ls.Add("0"); // Hardening modulus

      GSA.RunGWACommand(string.Join("\t", ls));
    }
  }

  public static partial class Conversions
  {
    public static bool ToNative(this StructuralMaterialSteel mat)
    {
      new GSAMaterialSteel() { Value = mat }.SetGWACommand(GSA);

      return true;
    }

    public static SpeckleObject ToSpeckle(this GSAMaterialSteel dummyObject)
    {
      if (!GSASenderObjects.ContainsKey(typeof(GSAMaterialSteel)))
        GSASenderObjects[typeof(GSAMaterialSteel)] = new List<object>();

      List<GSAMaterialSteel> materials = new List<GSAMaterialSteel>();

      string keyword = typeof(GSAMaterialSteel).GetGSAKeyword();
      string[] subKeywords = typeof(GSAMaterialSteel).GetSubGSAKeyword();

      string[] lines = GSA.GetGWARecords("GET_ALL," + keyword);
      List<string> deletedLines = GSA.GetDeletedGWARecords("GET_ALL," + keyword).ToList();
      foreach (string k in subKeywords)
        deletedLines.AddRange(GSA.GetDeletedGWARecords("GET_ALL," + k));

      // Remove deleted lines
      GSASenderObjects[typeof(GSAMaterialSteel)].RemoveAll(l => deletedLines.Contains((l as IGSASpeckleContainer).GWACommand));
      foreach (KeyValuePair<Type, List<object>> kvp in GSASenderObjects)
        kvp.Value.RemoveAll(l => (l as IGSASpeckleContainer).SubGWACommand.Any(x => deletedLines.Contains(x)));

      // Filter only new lines
      string[] prevLines = GSASenderObjects[typeof(GSAMaterialSteel)].Select(l => (l as IGSASpeckleContainer).GWACommand).ToArray();
      string[] newLines = lines.Where(l => !prevLines.Contains(l)).ToArray();

      foreach (string p in newLines)
      {
        GSAMaterialSteel mat = new GSAMaterialSteel() { GWACommand = p };
        mat.ParseGWACommand(GSA);
        materials.Add(mat);
      }

      GSASenderObjects[typeof(GSAMaterialSteel)].AddRange(materials);

      if (materials.Count() > 0 || deletedLines.Count() > 0) return new SpeckleObject();

      return new SpeckleNull();
    }
  }
}
