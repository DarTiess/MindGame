using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Leopotam.Serialization.Csv;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static GoogleSheets.ParseConst;

//using _Tools.PersistentValues;

namespace GoogleSheets
{
    public abstract class ParseHelper
    {
        public static void SaveObjectToResourcesInJson<T>(T instance, string name = default)
        {
#if UNITY_EDITOR
            string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            string fileName = name ?? typeof(T).Name;
            string path = $"Assets/Resources/GameData/{fileName}.json";
            File.WriteAllText(path, json);
            Debug.Log("PATH = " + path);
            AssetDatabase.Refresh();

#else
            string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            string fileName = name ?? typeof(T).Name;
            string path =  Application.persistentDataPath+$"{fileName}.json";
            File.WriteAllText(path, json);
            Debug.Log("PATH = " + path);
#endif
        }
        
        public static void SaveObjectToResourcesInTxt<T>(T instance, string name = default)
        {
#if UNITY_EDITOR
            string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            string path = "";
            string fileName = name ?? typeof(T).Name;
            path = $"Assets/Resources/GameData/{fileName}asd.txt";
            File.WriteAllText(path, json);
            Debug.Log("PATH = " + path);
            AssetDatabase.Refresh();
#endif
        }
        
        public static bool IsNewVersionBiggerThanOldForUnity(int newVersion)
        {
#if UNITY_EDITOR
            string path = "Assets/Resources/GameData/VersionFile.txt";
            if (File.Exists(path))
            {
                string txt = File.ReadAllText(path);
                int oldVer = int.Parse(txt, NumberStyles.Any, CultureInfo.InvariantCulture);

                if (newVersion > oldVer)
                {
                    txt = JsonConvert.SerializeObject(newVersion, Formatting.Indented);
                    File.WriteAllText(path, txt);
                    AssetDatabase.Refresh();
                    return true;
                }
   
                return false;
            }
            
            string newTxt = JsonConvert.SerializeObject(newVersion, Formatting.Indented);
            File.WriteAllText(path, newTxt);
            AssetDatabase.Refresh();
            return true;
#endif
            return false;
        }

    /*    public static bool IsNewVersionBiggerThanOldForRelease(int newVersion)
        {
            IntDataValueSavable savedVersion = new IntDataValueSavable("VersionFileKey");

            if (savedVersion.HasSaving())
            {
                if (newVersion > savedVersion.Value)
                {
                    savedVersion.Value = newVersion;
                    savedVersion.Save();
                    return true;
                }
   
                return false;
            }
            
            savedVersion.Value = newVersion;
            savedVersion.Save();
            return true;
        }*/

        public static string LoadString(string fileName)
        { 
            string path = $"GameData/{fileName}";
            TextAsset textFile = Resources.Load<TextAsset>(path);
            return textFile.ToString();
        }

        public static Dictionary<string, string> GetKeyedValues(string csv)
        {
            (Dictionary<string, string> dict, bool ok) = CsvReader.ParseKeyedValues(csv, true);
            return ok ? dict : null;
        }

        public static Dictionary<string, List<string>> GetKeyedListsByOwner(string csv, string ownerName = default)
        {
            (Dictionary<string, List<string>> dict, bool ok) = CsvReader.ParseKeyedLists(csv, true);

            if (dict[Key].Contains(Owner) && ownerName != default)
            {
                int ownerIndex = dict[Key].IndexOf(Owner);
                Dictionary<string, List<string>> selectedValuesDict = new Dictionary<string, List<string>>();
                foreach (var pair in dict)
                {
                    List<string> owners = pair.Value[ownerIndex].Split(' ').ToList();
                    if (pair.Value[ownerIndex] == Owner || owners.Any(o=> o == ownerName))
                        selectedValuesDict.Add(pair.Key, pair.Value);
                }

                return ok ? selectedValuesDict : null;
            }

            return ok ? dict : null;
        }

        public static object WriteDataInObjectByType(Type t, Dictionary<string, string> data,
            Dictionary<string, string> allCsvData, string owner = default)
        {
            FieldInfo[] fields = t.GetFields();
            object instance = Activator.CreateInstance(t);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    KeyValuePair<string, string> stat = data.FirstOrDefault(s => s.Key == field.Name);
                    Type itemType = field.FieldType.GetGenericArguments()[0];
                    if (allCsvData.TryGetValue(stat.Value, out string value))
                    {
                        try
                        {
                            Dictionary<string, List<string>> dict = GetKeyedListsByOwner(value, owner);
                            object values = WriteDataInListByType(itemType, dict, allCsvData, owner);
                            field.SetValue(instance, values);
                        }
                        catch (Exception e)
                        {
	                        Debug.Log(e);
                            Debug.Log("exception on -> " + stat.Value);
                            throw;
                        }
                       
                    }
                    else
                    {
                        Debug.Log("exception - no entry in csv dictionary - on -> " + stat.Value);
                    }
                }
                else
                {
                    KeyValuePair<string, string> stat = data.FirstOrDefault(s => s.Key == field.Name);
                    WriteStatInField(field, instance, stat.Value);
                }
            }

            return instance;
        }

        public static object WriteDataInListByType(Type t, Dictionary<string, List<string>> data, Dictionary<string, string> allCsvData, string owner = default)
        {
            Array instances;
//            Debug.Log(t.Name);
           
            if (data[Key].Any(val => val.Contains(ListIndex)))
            {
                int index = 0;
                KeyValuePair<string, List<string>> keyedList = data.FirstOrDefault(pair => pair.Key != Key);
                instances = Array.CreateInstance(t, keyedList.Value.Count - 1);
                foreach (string value in keyedList.Value)
                {
                    if (value == owner)
                        continue;
                    
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(t);
                    object instance = typeConverter.ConvertFromInvariantString(value);
                    instances.SetValue(instance, index);
                    index++;
                }
            }
            else
            {
                // Create an array of the required type and fill it with values of the required type
                instances = Array.CreateInstance(t, data.Count - 1);
                int index = 0;
                foreach (KeyValuePair<string, List<string>> keyedString in data)
                {
                    if (keyedString.Key == Key)
                        continue;

                    Dictionary<string, string> keyedValues = new();
                    for (var i = 0; i < data[Key].Count; i++)
                        keyedValues[data[Key][i]] = keyedString.Value[i];

                    owner = keyedString.Key;
                    object instance = WriteDataInObjectByType(t, keyedValues, allCsvData, owner);
                    instances.SetValue(instance, index);
                    index++;
                }
            }

            // Create a list of the required type, passing the values to the constructor
            Type genericListType = typeof(List<>);
            Type concreteListType = genericListType.MakeGenericType(t);
            object list = Activator.CreateInstance(concreteListType, new object[] { instances });
            return list;
        }

        private static void WriteStatInField(FieldInfo field, object instance, string stringValue)
        {
            try
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(field.FieldType);
                object convertedValue = typeConverter.ConvertFromInvariantString(stringValue);
                field.SetValue(instance, convertedValue);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Broken field -> " + field.Name);
                Debug.Log("Broken object -> " + instance);
                throw;
            }
        }
    }
}