﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Politechnikon.engine
{
    public class Logging
    {
        private Boolean DoLogging;
        public Logging() { DoLogging = true; }
        //klasa odpowiedzialna za generowanie logów
        public void GenerateLog(Exception e)
        {
            if (DoLogging)
            {
                if (!(Directory.Exists(@"logs"))) Directory.CreateDirectory(@"logs");
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"logs\\log_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");
                file.WriteLine(e.ToString());
                file.Close();
            }
        }
    }
}
