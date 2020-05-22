using System;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;
using SimplSharpLogger;

namespace JsonConfigManager
{
    public class JsonConfig
    {
        #region Properties

        private string _filePath { get; set; }
        private bool _isInitialized { get; set; }
        private JsonData _jsonData = new JsonData();
        private CCriticalSection _critSect = new CCriticalSection();

        public delegate void ReturnJsonValuesHandler(SimplSharpString key, SimplSharpString value, SimplSharpString type);
        public ReturnJsonValuesHandler ReturnJsonValues { get; set; }
        public delegate void FileBusyHandler(ushort fileBusy);
        public FileBusyHandler FileBusy { get; set; }

        #endregion

        #region S+ Methods

        /// <summary>
        /// Set file path for config file. Return 1 if successful, else 0.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>ushort</returns>
        public ushort Initialize(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                Debug.Log("File does not exist: " + _filePath, Debug.ErrorLevel.Error);
                _isInitialized = false;
                return 0;
            }
            _isInitialized = true;
            return 1;
        }

        /// <summary>
        /// Read and deserialize JSON file
        /// </summary>
        public void ReadConfig()
        {
            if (!_isInitialized)
            {
                Debug.Log("Not Initialized", Debug.ErrorLevel.Warning);
                return;
            }

            FileBusy(1);
            try
            {
                _critSect.Enter();
                var fileContents = File.ReadToEnd(_filePath, Encoding.ASCII);
                var jsonSignals = JsonConvert.DeserializeObject<JsonData>(fileContents);
                foreach (var sig in jsonSignals.Signals)
                {
                    ReturnJsonValues(sig.Key, sig.Value.ToString(), sig.Type.ToLower());
                }
                Debug.Log("File Read Success", Debug.ErrorLevel.None);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " occured reading from file: " + _filePath, Debug.ErrorLevel.Error);
            }
            finally
            {
                FileBusy(0);
                _critSect.Leave();
            }
        }

        /// <summary>
        /// Serialize and Write JSON config to file
        /// </summary>
        public void WriteConfig()
        {
            FileStream file = null;

            if (!_isInitialized)
            {
                Debug.Log("Not Initialized", Debug.ErrorLevel.Warning);
                return;
            }

            _critSect.Enter();
            FileBusy(1);
            try
            {
                file = new FileStream(_filePath, FileMode.Create);
                var jsonContents = JsonConvert.SerializeObject(_jsonData, Formatting.Indented);
                file.Write(jsonContents, Encoding.ASCII);
                Debug.Log("File Write Success", Debug.ErrorLevel.None);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " occured writing to file: " + _filePath, Debug.ErrorLevel.Error);
            }
            finally
            {
                if(file != null)
                    file.Close();
                FileBusy(0);
                _critSect.Leave();
            }
        }

        /// <summary>
        /// Populate _jsonData.Signals with JSON data set in Simpl
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="type">type</param>
        public void SetJsonValues(string key, string value, string type)
        {
            _jsonData.Signals.Add(new Signal { Key = key, Value = value, Type = type });
        }

        #endregion
    }
}
