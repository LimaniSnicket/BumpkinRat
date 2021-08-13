using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILevel 
{
  string LevelDataPath { get; }
  LevelData LevelData { get; }
}


