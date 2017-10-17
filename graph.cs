using System;
using System.IO;
using System.Collections.Generic;

namespace Oriented-graph
{
    /** Класс Graph реализует математический объект "граф", задаваемый списком смежности,
     * хранящим ориентированные ребра т.е. являющийся ориентированным графом. 
     */
    class Graph
    {
        /** Список vertexes_ вершин экземпляра графа хранит информацию о каждой вершине 
         * и исходящих из неё дугах (ориентированных ребрах).
         * Это единственное поле класса. 
         */
        private List<Node> vertexes_;

        /** Вложенный класс Node реализует единичный элемент списка vertexes_,
         * описывающий одну вершину графа.
         * Публичное поле name хранит название вершины, закрытый список вершин neighbours_ 
         * содержит названия вершин, к которым исходят дуги из данной вершины.
         */
        class Node
        {
            public string name;
            private List<string> neighbours_;

            /** Публичный конструктор Node(string) принимает строку вида:
             * a:b,c
             * где a это название новой вершины, b и c - названия всех вершин, к которым 
             * от неё исходят дуги (вершин-"соседей").
             */
            public Node(string parse)
            {
                char[] delims =
                {
                    ':',
                    ','
                };
                string[] parseResult = parse.Split(delims);
                name = parseResult[0];
                neighbours_ = new List<string>();
                for(int i = 1; i < parseResult.Length; i++)
                {
                    neighbours_.Add(parseResult[i]);
                }
                
            }          

            /** Публичный метод print() выводит название вершины и всех исходящих из неё дуг
             * на экран.
             */
            public void print()
            {
                if(neighbours_.Count == 0)
                {
                    Console.WriteLine("- Вершина {0} не имеет исходящих дуг.", name);
                    return;
                }

                Console.WriteLine("- Вершина {0}:", name);
                Console.WriteLine("Исходящие дуги:");
                foreach(var i in neighbours_)
                {
                    Console.WriteLine("{0} -> {1}", name, i);
                }
            }

            /** Метод addEdge(string) добавляет в список вершин-"соседей" вершину
             * с заданным именем, т.е. создает новую дугу, исходящую из this
             * в new Edge.
             */
            public void addEdge(string newEdge)
            {
                neighbours_.Add(newEdge);
            }

            /** Метод deleteEdge(string) удаляет из списка вершин-"соседей" this
             * все вхождения deletedEdge, т.е. удаляет все дуги, исходящие из this
             * в deletedEdge. 
             */
            public void deleteEdge(string deletedEdge)
            {
                while (neighbours_.Remove(deletedEdge)) ;
            }

            /** Метод findLoop() выполняет роль функции-предиката, 
             * возвращающей true в случае если вершина содержит себя в списке
             * вершин-"соседей", то есть в случае если вершина имеет петлю,
             * и false в случае если вершина не имеет петель.
             * Метод используется при поиске петель в графе, его вызов производится 
             * для каждой вершины в графе (для каждого элемента списка vertexes_).
             */
            public bool findLoop()
            {
                foreach(var i in neighbours_)
                {
                    if(name == i)
                    {
                        return true;
                    }
                }
                return false;
            }
        } /** Конец реализации класса вершины Node */

        /** Публичный конструктор класса Graph() создает "пустой" граф -
         * граф, не имеющий ни вершин, ни дуг, инициализируя поле vertexes_ пустым списком вершин.
         */
        public Graph()
        {
            vertexes_ = new List<Node>();
        }

        /** Публичный конструктор класса Graph(string) создает граф,
         * заданный файлом filename.
         * Файл имеет вид:
         * a:b,c
         * b:c,a
         * c:a,b
         * где a, b и с - вершины графа, а запись вида
         * a:b,c
         * означает что вершина a имеет две исходящие дуги: одна из a к b, другая из a к c.
         */
        public Graph(string filename)
        {
            vertexes_ = new List<Node>();
            StreamReader fileReader = new StreamReader(filename);
            string stringBuff;
            while( (stringBuff = fileReader.ReadLine()) != null )
            {
                addNode(stringBuff);
            }
        }

        /** Публичный конструктор Graph(Graph) реализует глубокое копирование
         * уже имеющегося объекта графа для независимого (не изменяющего состояние оригинала)
         * выполнения операций над копией.
         */
        public Graph(Graph source)
        {
            vertexes_ = new List<Node>();
            foreach(var i in source.vertexes_)
            {
                vertexes_.Add(i);
            }
        }

        /** Публичный конструктор Graph(int) создает полный граф с числами-названиями вершин по умолчанию. */
        public Graph(int num)
        {
            vertexes_ = new List<Node>();
            string buff;
            for(int i = 0; i < num; i++)
            {
                buff = Convert.ToString(i) + ":";
                for (int j = 0; j < num; j++)
                {
                    if (j != i)
                    {
                        buff += Convert.ToString(j) + ',';
                    }
                }
                if (buff[buff.Length - 1] == ',')
                {
                    buff = buff.Remove(buff.Length - 1);
                }
                addNode(buff);
            }
        }

        /** Метод класса Graph addNode(string) добавляет в список вершин новую, принимая строку из файла,
         * описывающего граф, или строку того же формата 
         * ([вершина-источник дуги]:[вершина-приемник],[вершина-приемник]).
         */
        public void addNode(string parse)
        {
            vertexes_.Add(new Node(parse));
        }

        /** Метод класса Graph deleteNode(string) удаляет из списка вершин 
         * вершину с заданным именем nodeName, а также все её упоминания в списках вершин-"соседей"
         * остальных вершин.
         */
        public void deleteNode(string nodeName)
        {
            for(var i = 0; i < vertexes_.Count; i++)
            {
                if(nodeName == vertexes_[i].name)
                {
                    /** При удалении вершины с заданным именем длина списка уменьшается на 1, 
                     * и счетчик i указывает теперь на следующий, необработанный по условию if
                     * элемент списка. Счетчик декрементируется после завершения работы 
                     * метода Remove, чтобы не пропустить удаление упоминаний искомой вершины во 
                     * впередистоящем элементе списка.
                     */
                    vertexes_.Remove(vertexes_[i--]);
                }
                else
                {
                    vertexes_[i].deleteEdge(nodeName);
                }
                
            }
        }

        /** Метод класса Graph addEdge(string, string) добавляет в число вершин-"соседей"
         * вершины sourceNode вершину destinationNode, т.е. создаёт дугу из вершины sourceNode
         * в вершину destinationNode.
         */
        public void addEdge(string sourceNode, string destinationNode)
        {
            /** Для поиска вершины-"источника" (вершины, из которой исходит создаваемая дуга)
             * применяется предикат "element => element.name == sourceNode",
             * применяемый методом Find ко всем элементам списка vertexes_ и 
             * метод Node.addEdge вызывается только для той вершины, которая имеет 
             * название sourceNode.
             */
            vertexes_.Find(element => element.name == sourceNode).addEdge(destinationNode);
        }

        /** Метод класса Graph deleteEdge(string, string) удаляет все имеющиеся в графе
         * дуги с началом в вершине sourceNode и концом в вершине destinationNode.
         */
        public void deleteEdge(string sourceNode, string destinationNode)
        {
            vertexes_.Find(element => element.name == sourceNode).deleteEdge(destinationNode);
        }

        /** Метод класса Graph print() выводит информацию обо всех вершинах 
         * (название и список вершин-"соседей") на экран.
         */
        public void print()
        {
            Console.WriteLine("Ориентированный граф:");
            foreach(var i in vertexes_)
            {
                i.print();
            }
            Console.WriteLine();
        }

        /** Метод класса Graph printLoopVertexes() выполняет поиск дуг в графе,
         * у которых вершина-"источник" и вершина-"приемник" совпадают,
         * и выводит их названия на экран.
         */
        public void printLoopVertexes()
        {
            Console.WriteLine("Поиск петель в ориентированном графе...");
            bool anyLoopsFound = false;
            foreach (var i in vertexes_)
            {
                if( i.findLoop() )
                {
                    Console.WriteLine("Вершина {0} имеет одну или несколько петель.", i.name);
                    anyLoopsFound = true;
                }
            }

            if(!anyLoopsFound)
            {
                Console.WriteLine("Граф не содержит петель.");
            }
            Console.WriteLine();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Graph myGraph = new Graph("input1.txt");
            myGraph.print();
            myGraph.printLoopVertexes();

            myGraph.deleteNode("a");
            myGraph.print();
            myGraph.printLoopVertexes();

            Graph secondGraph = new Graph(myGraph);
            secondGraph.deleteEdge("b", "b");
            secondGraph.deleteEdge("b", "c");
            secondGraph.print();
            secondGraph.printLoopVertexes();

            Graph thirdGraph = new Graph(5);
            thirdGraph.print();

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
