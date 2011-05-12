using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace CIMarketPriceFinder2007
{
    public partial class MarketPatternFinder : Form
    {
        Dictionary<int, MarketPattern> _marketPattern = new Dictionary<int, MarketPattern>();

        public MarketPatternFinder()
        {
            InitializeComponent();

            AddItemsOHLC(comboBoxOHLC1);
            AddItemsOHLC(comboBoxOHLC2);
            AddItemsOHLC(comboBoxOHLC3);
            AddItemsOHLC(comboBoxOHLC4);

            AddItemsTick(comboBoxDirection1);
            AddItemsTick(comboBoxDirection2);
            AddItemsTick(comboBoxDirection3);
            AddItemsTick(comboBoxDirection4);

        }
        public MarketPatternFinder(Dictionary<int, MarketPattern> marketPattern)
        {
            InitializeComponent();

            AddItemsOHLC(comboBoxOHLC1);
            AddItemsOHLC(comboBoxOHLC2);
            AddItemsOHLC(comboBoxOHLC3);
            AddItemsOHLC(comboBoxOHLC4);

            AddItemsTick(comboBoxDirection1);
            AddItemsTick(comboBoxDirection2);
            AddItemsTick(comboBoxDirection3);
            AddItemsTick(comboBoxDirection4);

            foreach (KeyValuePair<int, MarketPattern> KeyPair in marketPattern)
            {
                MarketPattern pattern = KeyPair.Value;
                switch (KeyPair.Key)
                {
                    case 1:
                        setComboBoxItemOHLC(comboBoxOHLC1, pattern.priceType);
                        setComboBoxItemTick(comboBoxDirection1, pattern.tickDirection);
                        checkBoxEnable1.Checked = true;
                        break;
                    case 2:
                        setComboBoxItemOHLC(comboBoxOHLC2, pattern.priceType);
                        setComboBoxItemTick(comboBoxDirection2, pattern.tickDirection);
                        checkBoxEnable2.Checked = true;
                        break;
                    case 3:                        
                        setComboBoxItemOHLC(comboBoxOHLC3, pattern.priceType);
                        setComboBoxItemTick(comboBoxDirection3, pattern.tickDirection);
                        checkBoxEnable3.Checked = true;
                        break;
                    case 4:
                        setComboBoxItemOHLC(comboBoxOHLC4, pattern.priceType);
                        setComboBoxItemTick(comboBoxDirection4, pattern.tickDirection);
                        checkBoxEnable4.Checked = true;
                        break;
                }
            }

            if (marketPattern.Count > 0) 
            {

            }
        }
        private void setComboBoxItemOHLC(ComboBox combo, PriceType priceType)
        {
            combo.Text = priceType.ToString();
        }

        private void setComboBoxItemTick(ComboBox combo, TickDirection tickDirection)
        {
            combo.Text = tickDirection.ToString();
        }
        private void AddItemsOHLC(ComboBox combo) 
        {
            combo.Items.Add(PriceType.Open.ToString());
            combo.Items.Add(PriceType.High.ToString());
            combo.Items.Add(PriceType.Low.ToString());
            combo.Items.Add(PriceType.Close.ToString());

            combo.Text = PriceType.Open.ToString();
        }

        private void AddItemsTick(ComboBox combo)
        {
            combo.Items.Add(TickDirection.Up.ToString());
            combo.Items.Add(TickDirection.NoChange.ToString());
            combo.Items.Add(TickDirection.Down.ToString());

            combo.Text = TickDirection.NoChange.ToString();
        }

        public enum PriceType
        {
            Open,
            High,
            Low ,
            Close
        } ;

        public enum TickDirection
        {
            Up = 1,
            Down = 2,
            NoChange = 3,
        } ;

        public Dictionary<int, MarketPattern> getMarketPatten()
        {
            return _marketPattern;
        }

        private void btnSavePattern_Click(object sender, EventArgs e)
        {
            _marketPattern.Clear();

            if (checkBoxEnable1.Checked)
            {
                MarketPattern newPattern1 = new MarketPattern();
                newPattern1.priceType = (PriceType)Enum.Parse(typeof(PriceType), comboBoxOHLC1.Text);
                newPattern1.tickDirection = (TickDirection)Enum.Parse(typeof(TickDirection), comboBoxDirection1.Text);
                _marketPattern.Add(1, newPattern1);

                if (checkBoxEnable2.Checked)
                {
                    MarketPattern newPattern2 = new MarketPattern();
                    newPattern2.priceType = (PriceType)Enum.Parse(typeof(PriceType), comboBoxOHLC2.Text);
                    newPattern2.tickDirection = (TickDirection)Enum.Parse(typeof(TickDirection), comboBoxDirection2.Text);
                    _marketPattern.Add(2, newPattern2);
                    if (checkBoxEnable3.Checked)
                    {
                        MarketPattern newPattern3 = new MarketPattern();
                        newPattern3.priceType = (PriceType)Enum.Parse(typeof(PriceType), comboBoxOHLC3.Text);
                        newPattern3.tickDirection = (TickDirection)Enum.Parse(typeof(TickDirection), comboBoxDirection3.Text);
                        _marketPattern.Add(3, newPattern3);
                        if (checkBoxEnable4.Checked)
                        {
                            MarketPattern newPattern4 = new MarketPattern();
                            newPattern4.priceType = (PriceType)Enum.Parse(typeof(PriceType), comboBoxOHLC4.Text);
                            newPattern4.tickDirection = (TickDirection)Enum.Parse(typeof(TickDirection), comboBoxDirection4.Text);
                            _marketPattern.Add(4, newPattern4);
                        }
                    }
                }
            }
            this.Close();
        }

        private void checkBoxEnable2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnable2.Checked)
            {
                if (!checkBoxEnable1.Checked) checkBoxEnable1.Checked = true;
            }
            if (!checkBoxEnable2.Checked)
            {
                if (checkBoxEnable3.Checked) checkBoxEnable3.Checked = false;
                if (checkBoxEnable4.Checked) checkBoxEnable4.Checked = false;
            }
        }

        private void checkBoxEnable3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnable3.Checked)
            {
                if (!checkBoxEnable1.Checked) checkBoxEnable1.Checked = true;
                if (!checkBoxEnable2.Checked) checkBoxEnable2.Checked = true;
            }
            if (!checkBoxEnable3.Checked)
            {
                if (checkBoxEnable4.Checked) checkBoxEnable4.Checked = false;
            }
        }

        private void checkBoxEnable4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnable4.Checked)
            {
                if (!checkBoxEnable1.Checked) checkBoxEnable1.Checked = true;
                if (!checkBoxEnable2.Checked) checkBoxEnable2.Checked = true;
                if (!checkBoxEnable3.Checked) checkBoxEnable3.Checked = true;
            }
        }

        private void checkBoxEnable1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxEnable1.Checked)
            {
                if (checkBoxEnable2.Checked) checkBoxEnable2.Checked = false;
                if (checkBoxEnable3.Checked) checkBoxEnable3.Checked = false;
                if (checkBoxEnable4.Checked) checkBoxEnable4.Checked = false;
            }
        }
        
    }
}
