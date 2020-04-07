using System;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;

namespace JsonConfigManager
{
    public class JsonConfig
    {
        #region Properties

        private string _filePath { get; set; }
        private bool _isInitialized { get; set; }
        private JsonData _jsonData = new JsonData();

        public delegate void ReturnJsonValuesHandler(SimplSharpString key, SimplSharpString value, SimplSharpString type);
        public ReturnJsonValuesHandler ReturnJsonValues { get; set; }
        public delegate void FileBusyHandler(ushort fileBusy);
        public FileBusyHandler FileBusy { get; set; }

        #endregion

        #region S+ Methods
        public ushort Initialize(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                Debug("File does not exist: " + _filePath, ErrorLevel.Error);
                _isInitialized = false;
                return 0;
            }
            _isInitialized = true;
            return 1;
        }

        public void ReadConfig()
        {
            if (!_isInitialized)
            {
                Debug("Not Initialized", ErrorLevel.Warning);
                return;
            }

            FileBusy(1);
            try
            {
                var fileContents = File.ReadToEnd(_filePath, Encoding.ASCII);
                var jsonSignals = JsonConvert.DeserializeObject<JsonData>(fileContents);
                foreach (var sig in jsonSignals.Signals)
                {
                    ReturnJsonValues(sig.Key, sig.Value.ToString(), sig.Type.ToLower());
                }
                Debug("File Read Success", ErrorLevel.None);
            }
            catch (Exception e)
            {
                Debug(e.Message + " occured reading from file: " + _filePath, ErrorLevel.Error);
            }
            finally
            {
                FileBusy(0);
            }
        }

        public void WriteConfig()
        {
            FileStream file = null;

            if (!_isInitialized)
            {
                Debug("Not Initialized", ErrorLevel.Warning);
                return;
            }

            FileBusy(1);
            try
            {
                file = new FileStream(_filePath, FileMode.Create);
                var jsonContents = JsonConvert.SerializeObject(_jsonData, Formatting.Indented);
                file.Write(jsonContents, Encoding.ASCII);
                Debug("File Write Success", ErrorLevel.None);
            }
            catch (Exception e)
            {
                Debug(e.Message + " occured writing to file: " + _filePath, ErrorLevel.Error);
            }
            finally
            {
                if(file != null)
                    file.Close();
                FileBusy(0);
            }
        }

        public void SetJsonValues(string key, string value, string type)
        {
            _jsonData.Signals.Add(new Signal { Key = key, Value = value, Type = type });
        }

        #endregion

        #region S# Methods
        private enum ErrorLevel { Notice, Warning, Error, None }

        private void Debug(string msg, ErrorLevel errLevel)
        {
            CrestronConsole.PrintLine(msg);
            if (errLevel != ErrorLevel.None)
            {
                switch (errLevel)
                {
                    case ErrorLevel.Notice:
                        ErrorLog.Notice(msg);
                        break;
                    case ErrorLevel.Warning:
                        ErrorLog.Warn(msg);
                        break;
                    case ErrorLevel.Error:
                        ErrorLog.Error(msg);
                        break;
                }
            }
        }
        #endregion
    }

    public class Signal
    {
        [JsonProperty("Key")]
        public string Key { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Value")]
        public object Value { get; set; }
    }

    public class JsonData
    {
        public List<Signal> Signals { get; set; }
        public JsonData()
        {
            Signals = new List<Signal>();
        }
    }
}
