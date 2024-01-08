using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Liquid.CommonUtils.Editor
{
    /// <summary>
    /// Symbols is a dynamic mapping of scripting define symbols,
    /// it provide easy edit methods to modify.
    /// </summary>
    public class Symbols
    {
        /// <summary>
        /// Platform corresponding to the current symbols.
        /// </summary>
        public NamedBuildTarget Platform = NamedBuildTarget.Unknown;
        private List<string> symbols = new List<string>();
        private const char delimiter = ';';

        /// <summary>
        /// Create a symbol object by platform.
        /// </summary>
        /// <param name="targetPlatform">Specified platform type.</param>
        public Symbols(NamedBuildTarget targetPlatform)
            : this(targetPlatform, PlayerSettings.GetScriptingDefineSymbols(targetPlatform)) 
        {}

        /// <summary>
        /// Serialize and output a list of all symbols.
        /// </summary>
        /// <returns>Symbols list.</returns>
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < symbols.Count; ++i)
            {
                result += symbols[i];
                if (i != symbols.Count - 1)
                    result += delimiter;
            }

            return result;
        }

        /// <summary>
        /// Save the current status of symbols in player settings of unity.
        /// </summary>
        public void Save()
        {
            PlayerSettings.SetScriptingDefineSymbols(
                Platform, ToString()
            );
        }

        /// <summary>
        /// Add a symbol for symbols.
        /// </summary>
        /// <param name="symbol">Symbol to operate.</param>
        public void Add(string symbol)
        {
            if (symbol == null)
                return;

            if (!Contains(symbol))
                symbols.Add(symbol);
        }

        /// <summary>
        /// Remove a symbol from symbols.
        /// </summary>
        /// <param name="symbol">Symbol to operate.</param>
        /// <returns>
        /// Returns true if the symbol successfully removed;
        /// otherwise, returns false.
        /// </returns>
        public bool Remove(string symbol)
        {
            if (symbol == null)
                return false;

            if (Contains(symbol))
                return symbols.Remove(symbol);
            else
                return false;
        }

        /// <summary>
        /// Judge whether the current symbol list contains the specified symbol.
        /// </summary>
        /// <param name="symbol">Symbol to search.</param>
        /// <returns>
        /// Returns true if the specified symbol is included;
        /// otherwise, returns false.
        /// </returns>
        public bool Contains(string symbol)
        {
            return symbols.Contains(symbol);
        }

        /// <summary>
        /// Create a symbol object by symbols string.
        /// </summary>
        /// <param name="targetPlatform">Specified platform type.</param>
        /// <param name="symbolStr">The symbols string provided by Unity editor.</param>
        private Symbols(NamedBuildTarget targetPlatform, string symbolStr)
        {
            symbols = new List<string>(
                symbolStr.Split(
                    delimiter,
                    StringSplitOptions.RemoveEmptyEntries
                )
            );

            Platform = targetPlatform;
            symbols = symbols.Distinct().ToList();
        }
    }
}