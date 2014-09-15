using System;
using UnityEditor;
using UnityEngine;

public class CFGImporter : EditorWindow
{
    private int _sheetIndex = 0;
    private TextAsset _ta;
    private ScriptableObject _so;

    [MenuItem("Tools/Import CFG")]
    static void _ImportCFG()
    {
        CFGImporter window = (CFGImporter)EditorWindow.GetWindow(typeof(CFGImporter));
        window.Show();
    }

    
    void OnGUI()
    {
        _sheetIndex = EditorGUILayout.IntField("sheet number: ", _sheetIndex);

        if (GUILayout.Button("根据文件生成通用配置描述文件", GUILayout.MaxWidth(200), GUILayout.Height(80)))
        {
            _ImportCommonCFGCSharp();
        }

        EditorGUILayout.Separator();

        _ta = EditorGUILayout.ObjectField("配置描述文件：", _ta, typeof(TextAsset), GUILayout.MinWidth(200)) as TextAsset;
        if (GUILayout.Button("根据文件生成通用配置数据文件", GUILayout.MaxWidth(200), GUILayout.Height(80)))
        {
            AssetUtil.SaveAsset(ScriptableObject.CreateInstance(_ta.name), _ta.name);
        }

        EditorGUILayout.Separator();

        _so = EditorGUILayout.ObjectField("配置文件：", _so, typeof(ScriptableObject), GUILayout.MinWidth(200)) as ScriptableObject;
        if (GUILayout.Button("根据文件导入通用配置数据", GUILayout.MaxWidth(200), GUILayout.Height(80)))
        {
            _ImportCommonCFGData();
        }
    }

    private void _ImportCommonCFGData()
    {
        string path = EditorUtility.OpenFilePanel("", "", "xlsx");
        if (string.IsNullOrEmpty(path))
            return;

        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        NPOI.SS.UserModel.IWorkbook book = null;
        if (path.EndsWith("xlsx"))
        {
            book = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
        }
        else if (path.EndsWith("xls"))
        {
            book = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
        }
        else
        {
            Debug.LogError("无法识别这种格式的excel文档！");
            fs.Close();
            fs.Dispose();
            fs = null;
            return;
        }

        NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt(_sheetIndex);

        Debug.Log("sheet.LastRowNum" + sheet.LastRowNum);
        int rows = sheet.LastRowNum;
        int cols = sheet.GetRow(0).LastCellNum;
        int c = 1;
        int r = 0;
        string tplName = "tpls";
        string tplClassName = _so.name + "Item";
        if (!string.IsNullOrEmpty(_so.name))
        {
            try
            {
                ScriptableObject so = _so;
                System.Reflection.FieldInfo tpls = so.GetType().GetField(tplName);
                Type itemType = tpls.FieldType.GetElementType();
                object[] array = Array.CreateInstance(itemType, rows) as object[];
                object item;
                for (int i = 0; i < rows; i++)
                {
                    r = i + 1;
                    item = System.Activator.CreateInstance(itemType);
                    for (int j = 0; j < cols; j++)
                    {
                        c = j;
                        string value = sheet.GetRow(r).GetCell(c).ToString().ToLower();
                        string sub = sheet.GetRow(0).GetCell(c).ToString();
                        System.Reflection.FieldInfo field = itemType.GetField(sub);

                        int outInt;
                        if (value == "true" || value == "false")
                        {
                            field.SetValue(item, value == "true" ? true : false);
                        }
                        else if (int.TryParse(value, out outInt))
                        {
                            Debug.Log(field + "," + item + "," + outInt);
                            field.SetValue(item, outInt);
                        }
                        else
                        {
                            field.SetValue(item, value);
                        }
                    }
                    array[i] = item;
                }
                tpls.SetValue(so, array);

                EditorUtility.SetDirty(_so);
                AssetDatabase.SaveAssets();
            }
            catch (Exception e)
            {
                Debug.LogError("导入错误：" + r + ", " + c + ";" + e.ToString());
                fs.Close();
                fs.Dispose();
                fs = null;
                return;
            }
            Debug.Log("导入成功！");

            fs.Close();
            fs.Dispose();
            fs = null;
        }
    }

    private void _ImportCommonCFGCSharp()
    {
        string path = EditorUtility.OpenFilePanel("", "", "xlsx");
        if (string.IsNullOrEmpty(path))
            return;

        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        NPOI.SS.UserModel.IWorkbook book = null;
        if (path.EndsWith("xlsx"))
        {
            book = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
        }
        else if (path.EndsWith("xls"))
        {
            book = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
        }
        else
        {
            Debug.LogError("无法识别这种格式的excel文档！");
            fs.Close();
            fs.Dispose();
            fs = null;
            return;
        }

        NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt(_sheetIndex);

        string clsName = System.IO.Path.GetFileNameWithoutExtension(path);
        string csharpPath = EditorUtility.SaveFilePanel("保存cs文件", "", clsName + ".cs", "cs");
        if (string.IsNullOrEmpty(csharpPath))
        {
            return;
        }
        string name = System.IO.Path.GetFileNameWithoutExtension(csharpPath);

        string tab = "\t";
        string br = "\n";
        string content = "using System.Collections.Generic;" + br
            + "using UnityEngine;" + br
            + "using System;" + br
            + br
            + "public class " + name + " : ScriptableObject" + br
            + "{" + br
            + tab + "public " + name + "Item[] tpls;" + br
            + "}" + br
            + br
            + "[System.Serializable]" + br
            + "public class " + name + "Item" + br
            + "{" + br;

        try
        {
            int cols = sheet.GetRow(0).LastCellNum;
            for (int i = 0; i < cols; i++)
            {
                string item = sheet.GetRow(0).GetCell(i).ToString();
                string value = sheet.GetRow(1).GetCell(i).ToString().ToLower();
                int outInt;
                if (value == "true" || value == "false")
                {
                    content += tab + "public bool " + item + ";" + br;
                }
                else if (int.TryParse(value, out outInt))
                {
                    content += tab + "public int " + item + ";" + br;
                }
                else
                {
                    content += tab + "public string " + item + ";" + br;
                }
            }
            content += "}";

            System.IO.File.WriteAllText(csharpPath, content, System.Text.Encoding.UTF8);
            //System.IO.StreamWriter define_sw = new System.IO.StreamWriter(csharpPath, false, System.Text.Encoding.UTF8);
            //define_sw.Write(content);
            //define_sw.Flush();
            //define_sw.Close();
            //define_sw.Dispose();
            //define_sw = null;
        }
        catch (Exception e)
        {
            Debug.LogError("生成类文件失败!" + e.StackTrace);

            fs.Close();
            fs.Dispose();
            fs = null;

            return;
        }
        Debug.Log("生成类文件成功：" + csharpPath);

        fs.Close();
        fs.Dispose();
        fs = null;
    }
}