﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;

namespace LoveStringH {
    public partial class LoveStringHForm : Form {
        static readonly InstalledFontCollection fonts = new InstalledFontCollection();

        bool stopTextChangedOnce;
        bool stopEscapeStyleChangedOnce;

        public LoveStringHForm() {
            InitializeComponent();

            cb_encoding.DisplayMember = "name";
            cb_escapeStyle.DisplayMember = "name";
            cb_transliterator.DisplayMember = "name";
            cb_fontDecode.DisplayMember = "Name";
            cb_fontTranslit.DisplayMember = "Name";

            cb_encoding.DataSource = Encoder.all;
            cb_transliterator.DataSource = Transliterator.all;
            cb_escapeStyle.DataSource = ((Encoder)cb_encoding.SelectedItem).styles;
            {
                FontFamily[] families = fonts.Families;
                cb_fontDecode.DataSource = cb_fontTranslit.DataSource = families;
                cb_fontDecode.SelectedIndex = cb_fontTranslit.SelectedIndex =
                    Array.IndexOf(families, this.Font.FontFamily);
                encodingFontChanged(this, new EventArgs());
                translitFontChanged(this, new EventArgs());
            }
            
            cb_escapeStyle.SelectedIndexChanged += new System.EventHandler(cb_escapeStyle_SelectedIndexChanged);
            cb_encoding.SelectedIndexChanged += new System.EventHandler(cb_encoding_SelectedIndexChanged);
            cb_transliterator.SelectedIndexChanged += new System.EventHandler(tb_roman_TextChanged);
            cb_fontDecode.SelectedIndexChanged += new System.EventHandler(encodingFontChanged);

            KeyPreview = true;

            stopTextChangedOnce = false;
            stopEscapeStyleChangedOnce = false;
        }

        private void tb_main_TextChanged(object sender, EventArgs e) {
            if (stopTextChangedOnce) return;
            stopTextChangedOnce = true;
            tb_byte.Text = ((Encoder)cb_encoding.SelectedItem).encode(
                tb_main.Text,
                ((Encoder.EscapeStyle)cb_escapeStyle.SelectedItem).escape);
            stopTextChangedOnce = false;
            ch_keepText.Checked = true;
        }

        private void tb_byte_TextChanged(object sender, EventArgs e) {
            if (stopTextChangedOnce) return;
            stopTextChangedOnce = true;
            tb_main.Text = ((Encoder)cb_encoding.SelectedItem).decode(tb_byte.Text);
            stopTextChangedOnce = false;
            ch_keepText.Checked = false;
        }

        private void cb_encoding_SelectedIndexChanged(object sender, EventArgs e) {
            Encoder.EscapeStyle[] styles = ((Encoder)cb_encoding.SelectedItem).styles;
            if (cb_escapeStyle.DataSource != styles) {
                stopEscapeStyleChangedOnce = true;
                cb_escapeStyle.DataSource = styles;
                stopEscapeStyleChangedOnce = false;
            }
            if (ch_keepText.Checked) tb_main_TextChanged(sender, e);
            else tb_byte_TextChanged(sender, e);
        }

        private void cb_escapeStyle_SelectedIndexChanged(object sender, EventArgs e) {
            if (stopEscapeStyleChangedOnce) return;
            tb_main_TextChanged(sender, e);
        }

        private void encodingFontChanged(object sender, EventArgs e) {
            tb_main.Font = new Font((FontFamily)cb_fontDecode.SelectedItem, (float)nud_fontsizeDecode.Value);
        }
        private void translitFontChanged(object sender, EventArgs e) {
            tb_nonroman.Font = new Font((FontFamily)cb_fontTranslit.SelectedItem, (float)nud_fontsizeTranslit.Value);
        }

        private void tb_roman_TextChanged(object sender, EventArgs e) {
            tb_nonroman.Text = ((Transliterator)cb_transliterator.SelectedItem).GetTranslit(tb_roman.Text);
        }

        private void form_KeyUp(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.Alt) {
                switch (e.KeyCode) {
                    case Keys.L:
                        tabcontrol_main.SelectedIndex = 1;
                        cb_transliterator.SelectedIndex = 0;
                        tb_roman.Focus();
                        e.Handled = true;
                        break;
                    case Keys.G:
                        tabcontrol_main.SelectedIndex = 1;
                        cb_transliterator.SelectedIndex = 1;
                        tb_roman.Focus();
                        e.Handled = true;
                        break;
                    case Keys.R:
                        tabcontrol_main.SelectedIndex = 1;
                        cb_transliterator.SelectedIndex = 2;
                        tb_roman.Focus();
                        e.Handled = true;
                        break;
                    case Keys.A:
                        tabcontrol_main.SelectedIndex = 1;
                        cb_transliterator.SelectedIndex = 3;
                        tb_roman.Focus();
                        e.Handled = true;
                        break;
                    case Keys.K:
                        tabcontrol_main.SelectedIndex = 1;
                        cb_transliterator.SelectedIndex = 4;
                        tb_roman.Focus();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void input_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                Clipboard.SetText(tb_nonroman.Text);
                tb_roman.SelectAll();
            }
        }
    }
}
