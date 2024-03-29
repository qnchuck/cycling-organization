﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using BiciklistickiSavez.Database;
using BiciklistickiSavez.CRUD;
using System.Globalization;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using BiciklistickiSavez.Views;
using System.Text.RegularExpressions;

namespace BiciklistickiSavez.ViewModels
{
    public class NoviTakmicarVM
    {
        public ObservableCollection<SystemModels.Models.Takmicar> Takmicari { get; set; }
        public SystemModels.Models.Takmicar Takmicar;
        public string nazivSaveza;

        public NoviTakmicarVM(List<SystemModels.Models.Takmicar> takmicari, string naziv)
        {
            Takmicari = new ObservableCollection<SystemModels.Models.Takmicar> (takmicari);
            nazivSaveza = naziv;
            AddTakmicarCommand = new RelayCommand<object>(AddTakmicar);
            UpdateTakmicarCommand = new RelayCommand<object>(UpdateTakmicarRow);
            RemoveTakmicarCommand = new RelayCommand<object>(RemoveTakmicarRow);
            PregledajBicikleCommand = new RelayCommand<object>(PregledajBicikle);
            this.TakmicarAdded += HandleTakmicarAdded;
        }
        public string JMBG { get; set; }
        public string Pol { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string NazivSaveza { get; set; }

        public event EventHandler TakmicarAdded;

        public event EventHandler ClosingTakmicari;

        public ICommand WindowClosingCommand { get; }
        public ICommand AddTakmicarCommand { get; set; }
        public ICommand PregledajBicikleCommand { get; set; }
        public ICommand UpdateTakmicarCommand { get; set; }
        public ICommand RemoveTakmicarCommand { get; set; }
        private void AddTakmicar(object parameter)
        {
            if (!AreRequiredFieldsFilled())
            {
                MessageBox.Show("Please fill out required fields");
                return;
            }
            if (JMBG.Length != 13)
            {
                MessageBox.Show("JMBG se sastoji od 13 cifara");
                return;
            }
            Takmicar = new SystemModels.Models.Takmicar
            {
                JMBG = JMBG,
                Ime = Ime,
                Prezime = Prezime,
                Pol = Pol,
                NazivSaveza = nazivSaveza,

            };
            TakmicarCRUD.Instance.Create(Takmicar);
            TakmicarAdded?.Invoke(this, EventArgs.Empty);
        }
        private void UpdateTakmicarRow(object parameter)
        {
            SystemModels.Models.Takmicar takmicar = parameter as SystemModels.Models.Takmicar;
            if (takmicar != null)
            {

                TakmicarCRUD.Instance.Modify(takmicar);
            }
        } 
        
        private void RemoveTakmicarRow(object parameter)
        {
            SystemModels.Models.Takmicar takmicar = parameter as SystemModels.Models.Takmicar;
            if (takmicar != null)
            {

                TakmicarCRUD.Instance.DeleteTakmicar(takmicar.JMBG);
            }
            Takmicari.Clear();
            TakmicarCRUD.Instance.GetAll().Where(t => t.NazivSaveza == nazivSaveza).ToList().ForEach(tt => Takmicari.Add(tt));
        }
        private void PregledajBicikle(object parameter)
        {
            SystemModels.Models.Takmicar takmicar = (SystemModels.Models.Takmicar)parameter;
            if (takmicar != null)
            {
                BicikliPregledVM bicikliPregledVM = new BicikliPregledVM(takmicar.Bicikli, takmicar.JMBG);
                BicikliPregled bicikliW = new BicikliPregled(bicikliPregledVM);
                bicikliW.ClosingEvents += OsveziTabelu;
                bicikliW.ShowDialog();
            }
        }
        private bool AreRequiredFieldsFilled()
        {
            return  !string.IsNullOrEmpty(Ime) && !string.IsNullOrEmpty(Prezime)
                && (Pol.ToUpper()=="M"||Pol.ToUpper()=="Z");
        }
        private void OsveziTabelu(object sender, EventArgs e)
        {
            Takmicari.Clear();
            TakmicarCRUD.Instance.GetAll().Where(t => t.NazivSaveza == nazivSaveza).ToList().ForEach(tt=> Takmicari.Add(tt));
        }
        private void HandleTakmicarAdded(object sender, EventArgs e)
        {
            Takmicari.Add(Takmicar);
        }
        

    }
    
}
