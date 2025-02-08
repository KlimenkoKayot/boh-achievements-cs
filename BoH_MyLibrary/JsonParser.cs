using System;

namespace BoH_MyLibrary
{
    public static class JsonParser
    {
        private enum State
        {
            Start,
            InObject,
            InKey,
            InValue,
            InString,
            InBoolOrNum,
            End,
        }

        // C:\Users\georg\AppData\Local\Temp\Temp1_data.zip\data\BoH\core\achievements\a_salons.json

        /// <summary>
        /// Реализация в виде конечного автомата
        /// </summary>
        /// <param name="json">Строка с данными</param>
        /// <returns>Возвращает Json обьект в виде словаря</returns>
        /// <exception cref="Exception">Ошибка, грустим(</exception>
        public static Dictionary<string, string> Deserialize(string json)
        {
            json.Trim();
            json = json.Replace("\n", "");

            Dictionary<string, string> dict = [];
            State curState = State.Start;
            string buffer = "", key = "";
            int cntStartValueSymbol = 0;
            char startValueSymbol;
            Stack<char> st = [];
            Stack<char> start = [];
            foreach (char symbol in json)
            {
                if (symbol == ' ' && curState != State.InValue && curState != State.InString)
                {
                    continue;
                }
                // реализация конечного автомата    
                if (symbol == '[' || symbol == '{')
                {
                    start.Push(symbol);
                    //Console.WriteLine($"{symbol} {curState} {start.Count()}");
                }
                if (symbol == '}' || symbol == ']')
                {
                    start.Pop();
                    //Console.WriteLine($"{symbol} {curState} {start.Count()}");
                }
                switch (curState)
                {
                    case State.Start:
                        {
                            if (symbol == '{')
                            {
                                curState = State.InObject;
                            }
                            else if (!char.IsWhiteSpace(symbol))
                            {
                                throw new Exception($"Unexpected symbol '{symbol}' in state {curState}");
                            }
                            break;
                        }

                    case State.InObject:
                        {
                            if (start.Count() == 0)
                            {
                                curState = State.End;
                            } 
                            else if (symbol == '"')
                            {
                                curState = State.InKey;
                            }
                            else if (symbol == ':')
                            {
                                curState = State.InValue;
                            }
                            else if (!char.IsWhiteSpace(symbol) && symbol != ',')
                            {
                                throw new Exception($"Unexpected symbol '{symbol}' in state {curState}");
                            }
                            break;
                        }
                    case State.InKey:
                        {
                            if (symbol == '"')
                            {
                                key = buffer;
                                buffer = "";
                                curState = State.InObject;
                            }
                            else
                            {
                                buffer += symbol;
                            }
                            break;
                        }
                    case State.InValue:
                        {
                            if (st.Count() == 0)
                            {
                                if (symbol == ' ') continue;
                                
                                if (symbol == '"')
                                {
                                    curState = State.InString;
                                    break;
                                } 
                                else if (symbol != '[' && symbol != '{')
                                {
                                    buffer += symbol;
                                    curState = State.InBoolOrNum;
                                    break;
                                }
                            }
                            if (symbol == '[' || symbol == '{')
                            {
                                st.Push(symbol);
                            }
                            else if (symbol == '}')
                            {
                                if (st.Peek() == '{')
                                {
                                    st.Pop();
                                }
                            }
                            else if (symbol == ']')
                            {
                                if (st.Peek() == '[')
                                {
                                    st.Pop();
                                }
                            }
                            buffer += symbol;
                            if (st.Count() == 0)
                            {
                                dict[key] = buffer.Trim();
                                buffer = "";
                                curState = State.InObject;
                            }
                            //if (symbol == '{' ||  symbol == '}' || symbol == '[' || symbol == ']') Console.WriteLine($"\t{symbol} {st.Count()}");
                            break;
                        }
                    case State.InString:
                        {
                            if (symbol == '"')
                            {
                                dict[key] = buffer.Trim();
                                buffer = "";
                                curState = State.InObject;
                            }
                            else
                            {
                                buffer += symbol;
                            }
                            break;
                        }
                    case State.InBoolOrNum:
                        {
                            if (symbol == '}' || symbol == ',')
                            {
                                dict[key] = buffer.Trim();
                                buffer = "";
                                curState = State.InObject;
                            }
                            else if (symbol != ' ')
                            {
                                buffer += symbol;
                            }
                            break;
                        }
                    case State.End:
                        {
                            // Завершение парсинга
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Unknown state: {curState}");
                        }
                }
            
                //Console.WriteLine($"| {key} | {curState} | {st.Count()} ");
            }
            return dict;
        }

        // тут вообще каряво JsonObject делать, а не Dictionary<string, string>
        // но я пишу это в сжатые сроки и не хочу париться (поставьте 10)
        public static List<JsonObject>? DeserializeArray(string json)
        {
            if (!(json[0] == '[' && json[json.Length-1] == ']'))
            {
                throw new Exception("json string is not array");
            }
            json = json.Substring(1, json.Length - 2).Trim();
            List<JsonObject> objList = [];
            Stack<char> st = [];
            string buffer = "";
            List<string> jsons = [];
            for (int i = 0; i < json.Length; i++)
            {
                if (st.Count() == 0 && json[i] == ',')
                {
                    continue;
                }
                if (json[i] == '{')
                {
                    if (st.Count() == 0 && buffer != "")
                    {
                        //Console.WriteLine($"ADDED: {buffer.Length}");
                        jsons.Add(buffer);
                        buffer = "";
                    }
                    st.Push(json[i]);
                    //Console.WriteLine($"{json[i]} {st.Count()}");
                } else if (json[i] == '}')
                {
                    st.Pop();
                    //Console.WriteLine($"{json[i]} {st.Count()}");
                }
                if (st.Count() == 0 && (json[i] == ' ')) continue;
                buffer += json[i];
            }
            if (buffer != "")
            {
                jsons.Add(buffer);
            }

            foreach (string curJson in jsons)
            {
                objList.Add(new JsonObject(Deserialize(curJson)));
            }
            //Console.WriteLine($"LEN : {objList.Count()}");
            //foreach (JsonObject cur in objList)
            //{
            //    foreach (string str in cur.GetAllFields())
            //    {
            //        Console.WriteLine($"{str} : {cur.GetField(str)}");
            //    }
            //    Console.WriteLine();
            //}
            // реализация конечного автомата + массива
            return objList;
        }
    }
}
