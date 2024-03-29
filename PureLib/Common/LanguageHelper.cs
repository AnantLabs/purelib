﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.International.Converters;
using Microsoft.VisualBasic;

namespace PureLib.Common {
    public static class LanguageHelper {
        private static ChineseConverter _converter;
        private static ChineseConverter _loadedConverter {
            get {
                if (_converter == null)
                    _converter = new ChineseConverter();
                return _converter;
            }
        }

        public static string ResolveUnreadableCodes(this string text, Encoding src, Encoding dst) {
            return dst.GetString(src.GetBytes(text));
        }

        public static string ToProperCase(this string text) {
            return Strings.StrConv(text, VbStrConv.ProperCase);
        }

        public static string ToNarrow(this string wide) {
            return Strings.StrConv(wide, VbStrConv.Narrow);
        }

        public static string ToWide(this string narrow) {
            return Strings.StrConv(narrow, VbStrConv.Wide);
        }

        public static string ToSimplifiedChinese(this string traditional) {
            return _loadedConverter.ToSimplifiedChinese(traditional);
        }

        public static string ToTraditionalChinese(this string simplified) {
            return _loadedConverter.ToTraditionalChinese(simplified);
        }

        public static string ToKatakana(this string hiragana) {
            return KanaConverter.HiraganaToKatakana(hiragana);
        }

        public static string ToHiragana(this string katakana) {
            return KanaConverter.KatakanaToHiragana(katakana);
        }

        public static string ParseRomaji(this string romaji) {
            return KanaConverter.RomajiToHiragana(romaji);
        }

        private class ChineseConverter {
            public const string AdditionalTcScPairName = "additionalTcScPair";
            private const char pairsSeparator = ',';
            private const char pairSeparator = ':';
            private Dictionary<string, string> tcScMaps = new Dictionary<string, string>();

            public ChineseConverter() {
                string pairString = ConfigurationManager.AppSettings[AdditionalTcScPairName];
                if (!pairString.IsNullOrEmpty()) {
                    string[] scTcPairs = pairString.Split(pairsSeparator);
                    foreach (string scTcPair in scTcPairs) {
                        string[] parts = scTcPair.Split(pairSeparator);
                        tcScMaps.Add(parts.First(), parts.Last());
                    }
                }
            }

            public string ToSimplifiedChinese(string traditional) {
                foreach (var p in tcScMaps) {
                    traditional = traditional.Replace(p.Key, p.Value);
                }
                return Strings.StrConv(traditional, VbStrConv.SimplifiedChinese);
            }

            public string ToTraditionalChinese(string simplified) {
                foreach (var p in tcScMaps) {
                    simplified = simplified.Replace(p.Value, p.Key);
                }
                return Strings.StrConv(simplified, VbStrConv.TraditionalChinese);
            }
        }
    }
}
