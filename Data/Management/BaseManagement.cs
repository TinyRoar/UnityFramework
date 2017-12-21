using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace TinyRoar.Framework
{
    public abstract class BaseManagement<T> : Singleton<T> where T : new()
    {

        protected static string PATH_XML
        {
            get;
            set;
        }

        protected static string PATH_BIN
        {
            get;
            set;
        }

        protected static string PATH_CRYPT
        {
            get;
            set;
        }

        protected bool SerializeInstantly
        {
            get;
            private set;
        }

        private int tileIndex
        {
            get;
            set;
        }

        private SaveMethod useSaveMethod
        {
            get;
            set;
        }

        private TextAsset textAsset
        {
            get;
            set;
        }

        private bool _doSaving = false;
        protected ManagementType Management;

        protected enum ManagementType
        {
            None,
            Pair,
        }

        protected bool doSaving
        {
            set
            {
                if (_doSaving == value)
                    return;
                _doSaving = value;

                if (Updater.Instance != null)
                    Updater.Instance.OnLateUpdate -= SaveEndOfFrame;

                if (_doSaving)
                {
                    time = Time.time;
                    if(Updater.Instance != null)
                        Updater.Instance.OnLateUpdate += SaveEndOfFrame;
                }
            }
        }

        private float time = 0;
        private float waitMinSec = 1;
        private float waitMaxSec = 2;
        private void SaveEndOfFrame()
        {
            if (!_doSaving)
            {
                Updater.Instance.OnLateUpdate -= SaveEndOfFrame;
                return;
            }

            // if enough rest-time in this frame and wait min. 1 sec and max 5 sec
            if (Time.time > time + waitMaxSec || (Time.time > time + waitMinSec && Time.deltaTime > 1f / 30f))
            {
                // do saving...
                DoSaving();
            }
        }

        // do saving logic
        private void DoSaving()
        {
            if (!_doSaving)
                return;

            // disable LateUpdate
            _doSaving = false;

            if (Updater.Instance != null)
                Updater.Instance.OnLateUpdate -= SaveEndOfFrame;

                // save
                switch (Management)
            {
                case ManagementType.Pair:
                    this.Serialize<PairCollection>(typeof(Pair));
                    break;
            }

            if(InitManager.Instance != null && InitManager.StaticDebug)
                Debug.LogWarning("Save " + Management);

        }

        public BaseManagement()
        {
            SerializeInstantly = true;
            if (useSaveMethod == SaveMethod.None)
                useSaveMethod = GameConfig.Instance.UseSaveMethod;
        }

        protected BaseCollection Collection { get; private set; }

        public void SetSaveMethod(SaveMethod saveMethod)
        {
            this.useSaveMethod = saveMethod;
        }

        public void SetSerializeInstantly(bool value)
        {
            this.SerializeInstantly = value;
        }

        // Serialize
        public void Serialize<U>(Type type) where U : BaseCollection
        {
            if (Collection == null)
            {
                UnityEngine.Debug.Log("Collection not init");
                return;
            }

            Thread thread = new Thread(() =>
            {
                if (useSaveMethod == SaveMethod.Xml)
                {
                    DataStorage.Instance.SerializeXml<U>(PATH_XML, Collection, type);
                }
                else
                {
                    DataStorage.Instance.SerializeBinary(PATH_BIN, Collection.Items);
                    Encrypt.Instance.EncryptFile(PATH_BIN, PATH_CRYPT);
                    DataStorage.Instance.DeleteFile(PATH_BIN);
                }
            });
            thread.Start();
        }

        // Deserialize
        public void Deserialize<U>(Type type) where U : new()
        {
            if (useSaveMethod == SaveMethod.Xml)
            {
                Collection = DataStorage.Instance.DeserializeXml<U>(PATH_XML, type) as BaseCollection;
            }
            else if (useSaveMethod == SaveMethod.TextAsset)
            {
                Collection = DataStorage.Instance.DeserializeXmlContent<U>(textAsset.text, type) as BaseCollection;
            }
            else
            {
                U iCol = new U();
                Collection = iCol as BaseCollection;
                if (DataStorage.Instance.FileExists(PATH_CRYPT))
                {
                    Encrypt.Instance.DecryptFile(PATH_CRYPT, PATH_BIN);
                    Collection.Items = DataStorage.Instance.DeserializeBinary<ArrayList>(PATH_BIN);
                    DataStorage.Instance.DeleteFile(PATH_BIN);
                }
            }
        }

        // Set Path
        public void SetPath(string fileName, bool PathToUserData = true, string extension = "")
        {
            //Debug.Log("SetPath" + fileName);
            string xmlFileName = fileName + ".xml";
            string binaryFileName = fileName + ".dec";
            if (extension != "")
            {
                xmlFileName = fileName + "." + extension;
                binaryFileName = fileName + "." + extension;
            }

            if (PathToUserData)
            {
                PATH_XML = Application.persistentDataPath + "/" + xmlFileName;
                PATH_BIN = Application.persistentDataPath + "/" + fileName + ".bin";
                PATH_CRYPT = Application.persistentDataPath + "/" + binaryFileName;
            }
            else
            {

                PATH_XML = xmlFileName;
                PATH_BIN = fileName + ".bin";
                PATH_CRYPT = binaryFileName;
            }
        }

        public void SetTextAsset(TextAsset textAsset)
        {
            this.textAsset = textAsset;
        }

        // Clear
        public void Clear()
        {
            this.Collection.Items.Clear();
        }

        // Clear Collection and delete files -> Reset all
        public void Reset()
        {
            DataStorage.Instance.DeleteFile(PATH_XML);
            DataStorage.Instance.DeleteFile(PATH_BIN);
            DataStorage.Instance.DeleteFile(PATH_CRYPT);

            this.Clear();
        }

        // Get Loop List
        public U GetNextTile<U>() where U : BaseElement
        {
            if (this.Collection.Count > tileIndex)
                return (U) this.Collection.Items[tileIndex++];
            return null;
        }

        // Reset Loop Index = 0
        public void ResetTileLoopIndex()
        {
            tileIndex = 0;
        }

        public ArrayList Items
        {
            get { return Collection.Items; }
        }

        public void ForceSaving()
        {
            DoSaving();
        }
    }
}