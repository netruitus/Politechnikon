using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Politechnikon.engine
{
    public class XMLParser
    {
        //deklaracja zmiennych
        private XDocument doc;
        private String FilePath;
        private String Reply;

        public XMLParser(String fileName)
        {
            //konstruktor - ładowanie ścieżek etc.
            Reply = null;
            this.FilePath = "Resources\\data\\" + fileName;
            doc = new XDocument();
            doc = XDocument.Load(@FilePath);
        }
        
        public String getElementByAttribute(String AttributeName, String AttributeValue, String ElementToGet){
            //szukanie konkretnego węzła i w nim konkretnego elementu po atrybucie
            Reply = null;
            var nodes = doc.Descendants().Where(x => x.Attribute(AttributeName) != null);
            var node = nodes.Where(x => x.Attribute(AttributeName).Value.Contains(AttributeValue)).FirstOrDefault();
            var ValNode = node.Descendants().Where(x => x.Name == ElementToGet).FirstOrDefault();
            Reply = ValNode.Value;
            if (Reply != null) return Reply;
            else return "";
        }

        public String getNodeAttributeValueByItsElementValue(String AttributeName, String ElementName, String ElementValue)
        {
            //szukanie konkretnego węzła, w którym szukamy konkretnego atrybutu dla jednego znanego nam elementu znajdującego się w tym węźle
            Reply = null;
            var nodesWithAttribute = doc.Descendants().Where(x => x.Attribute(AttributeName) != null);
            Reply = nodesWithAttribute.Where(x => x.Element(ElementName).Value == ElementValue).FirstOrDefault().Attribute(AttributeName).Value;
            if (Reply != null) return Reply;
            else return "";
        }

        public String getNodeAttributeValueByItsElementValue(String AttributeName, String FirstElementName, String FirstElementValue, String SecondElementName, String SecondElementValue)
        {
            //szukanie konkretnego węzła, w którym szukamy konkretnego atrybutu dla dwóch znanych nam elementów znajdujących się w tym węźle
            Reply = null;
            var nodesWithAttribute = doc.Descendants().Where(x => x.Attribute(AttributeName) != null);
            Reply = nodesWithAttribute.Where(x => x.Element(FirstElementName).Value == FirstElementValue &&
                x.Element(SecondElementName).Value == SecondElementValue).FirstOrDefault().Attribute(AttributeName).Value;
            if (Reply != null) return Reply;
            else return "";
        }

    }
}
