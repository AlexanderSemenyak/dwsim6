﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWSIM.Interfaces;
using DWSIM.Interfaces.Enums.GraphicObjects;
using DWSIM.Thermodynamics.BaseClasses;
using Eto.Drawing;
using Eto.Forms;
using c = DWSIM.UI.Shared.Common;

using cv = DWSIM.SharedClasses.SystemsOfUnits.Converter;
using DWSIM.Thermodynamics.Streams;
using DWSIM.UI.Desktop.Shared;
using DWSIM.UI.Shared;

using DWSIM.ExtensionMethods;
using System.IO;
using DWSIM.Thermodynamics.PropertyPackages;

namespace DWSIM.UI.Desktop.Editors
{
    public class SimulationSetupWizard
    {

        public IFlowsheet flowsheet;

        private int Width = 800;
        private int Height = 500;

        private bool hasLowPressure = false;
        private bool hasHC = false;
        private bool hasHCW = false;
        private bool hasHCWI = false;
        private bool hasPolarChemicals = false;
        private bool hasRefrigeration = false;
        private bool hasSingleCompoundWater = false;
        private bool hasElectrolytes = false;
        private bool hasTwoLiquids = false;
        private bool hasSolids = false;

        public SimulationSetupWizard(IFlowsheet fs)
            : base()
        {

            flowsheet = fs;
        }

        public void DisplayPage0(Control owner = null)
        {

            var page1 = new WizardPage();

            page1.hasBackButton = false;
            page1.hasCancelButton = true;
            page1.hasNextButton = true;
            page1.hasFinishButton = false;

            page1.cancelAction = () => page1.Close();
            page1.nextAction = () => { page1.Close(); DisplayPage1(); };

            page1.Title = "Simulation Setup Wizard";
            page1.HeaderTitle = "Simulation Setup Wizard";
            page1.HeaderDescription = "Welcome!";
            page1.FooterText = "";

            page1.Init(Width, Height);

            var dl = c.GetDefaultContainer();
            dl.Height = Height;
            dl.Width = Width;

            dl.CreateAndAddLabelRow2("Welcome to the Simulation Setup wizard. In the next steps you'll be able to configure the main simulation settings using a simplifed approach.");
            dl.CreateAndAddLabelRow2("You can close this wizard at your will. Many other (advanced) settings can be configured by using the appropriate editors, which can be found in the 'Setup' menu item.");
            dl.CreateAndAddLabelRow2("Click 'Next' to continue.");

            page1.ContentContainer.Add(dl);
            page1.ShowModal(owner);
        
        }

        private void DisplayPage1()
        {

            var page1 = new WizardPage();

            page1.hasBackButton = true;
            page1.hasCancelButton = true;
            page1.hasNextButton = true;
            page1.hasFinishButton = false;

            page1.cancelAction = () => page1.Close();
            page1.backAction = () => { page1.Close(); DisplayPage0(); };
            page1.nextAction = () => { page1.Close(); DisplayPage2(); };

            page1.Title = "Simulation Setup Wizard";
            page1.HeaderTitle = "Step 1 - Add Compounds";
            page1.HeaderDescription = "Select the compounds to add to the simulation. If your compound is not on the list, you can add\nother compounds using the Compound Creator Wizard ('Setup' > 'Compounds' > 'Compound Tools' > 'Compound Creator Wizard').";
            page1.FooterText = "";

            page1.Init(Width, Height);

            var tl = new TableLayout() { Width = Width, Height = Height };

            new Compounds(flowsheet, tl);

            page1.ContentContainer.Add(tl);
            page1.ShowModal();

        }

        private void DisplayPage2()
        {

            var page = new WizardPage();

            page.hasBackButton = true;
            page.hasCancelButton = true;
            page.hasNextButton = true;
            page.hasFinishButton = false;

            page.cancelAction = () => { page.Close(); };
            page.backAction = () => { page.Close(); DisplayPage1(); };
            page.nextAction = () => { page.Close(); DisplayPage3(); };

            page.Title = "Simulation Setup Wizard";
            page.HeaderTitle = "Step 2 - Process Model details";
            page.HeaderDescription = "Configure process model details.";
            page.FooterText = "";

            page.Init(Width, Height);

            var dl = c.GetDefaultContainer();
            
            dl.CreateAndAddLabelRow("Process Details");
            dl.CreateAndAddLabelRow2("Check/uncheck boxes according to your process charateristics and DWSIM will choose the best thermodynamic model setup for your simulation.");
            dl.CreateAndAddLabelRow2("Please check the minimum amount of boxes as possible, avoiding redundancy and/or incompatible items.");
            dl.CreateAndAddLabelRow2("If you prefer to setup the Property Packages manually, close this wizard and go to 'Setup' > 'Basis'.");

            dl.CreateAndAddLabelRow("General Information");

            dl.CreateAndAddCheckBoxRow("My process can be modeled using the Ideal Gas law for vapor phase and Ideal Solution Theory for liquid phase", hasLowPressure, (sender, e) => hasLowPressure = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("My process deals with Hydrocarbons only", hasHC, (sender, e) => hasHC = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("My process has Hydrocarbons and Water at higher pressures", hasHCW, (sender, e) => hasHCW = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("My process has Hydrocarbons and Water and they can be considered immiscible", hasHCWI, (sender, e) => hasHCWI = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("My process has polar chemicals", hasPolarChemicals, (sender, e) => hasPolarChemicals = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("My process deals with a refrigeration cycle", hasRefrigeration, (sender, e) => hasRefrigeration = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("This is a single Water/Steam simulation", hasSingleCompoundWater, (sender, e) => hasSingleCompoundWater = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("I'm simulating a process which involves aqueous electrolytes", hasElectrolytes, (sender, e) => hasElectrolytes = sender.Checked.GetValueOrDefault());

            dl.CreateAndAddLabelRow("Expected Phases");
            dl.CreateAndAddLabelRow2("The following options are mutually exclusive:");

            dl.CreateAndAddCheckBoxRow("I'm expecting to deal with two liquid phases in this simulation", hasTwoLiquids, (sender, e) => hasTwoLiquids = sender.Checked.GetValueOrDefault());
            dl.CreateAndAddCheckBoxRow("I'm expecting to deal with solids in this simulation", hasSolids, (sender, e) => hasSolids = sender.Checked.GetValueOrDefault());

            if (Application.Instance.Platform.IsGtk)
            {
                page.ContentContainer.Add(new Scrollable { Content = dl, Border = BorderType.None, Height = Height, Width = Width });
            }
            else
            {
                dl.Height = Height;
                dl.Width = Width;
                page.ContentContainer.Add(dl);
            }
            page.ShowModal();

        }

        private void DisplayPage3() {

            SetupPropertyPackage();
            SetupFlashAlgorithm();

            var page = new WizardPage();

            page.hasBackButton = true;
            page.hasCancelButton = true;
            page.hasNextButton = false;
            page.hasFinishButton = true;

            page.cancelAction = () => { page.Close(); };
            page.finishAction = () => { page.Close(); };
            page.backAction = () => { page.Close(); DisplayPage2(); };

            page.Title = "Simulation Setup Wizard";
            page.HeaderTitle = "Step 3 - Other Settings";
            page.HeaderDescription = "Configure miscellaneous simulation settings.";
            page.FooterText = "Click 'Finish' to close this window and start building your process model.";

            page.Init(Width, Height);

            var dl = c.GetDefaultContainer();
            dl.Height = Height;
            dl.Width = Width;

            c.CreateAndAddLabelRow(dl, "General");

            c.CreateAndAddStringEditorRow(dl, "Simulation Name", flowsheet.FlowsheetOptions.SimulationName, (sender, e) => flowsheet.FlowsheetOptions.SimulationName = sender.Text);

            c.CreateAndAddLabelRow2(dl, "The simulation name will be used for report identification and file name during saving.");

            var avunits = flowsheet.AvailableSystemsOfUnits.Select((x) => x.Name).ToList();

            c.CreateAndAddLabelRow(dl, "System of Units");
            c.CreateAndAddLabelRow2(dl, "Select the System of Units to be used on this simulation.");

            c.CreateAndAddDropDownRow(dl, "System of Units", avunits, avunits.IndexOf(flowsheet.FlowsheetOptions.SelectedUnitSystem.Name), (sender, e) =>
            {
                flowsheet.FlowsheetOptions.SelectedUnitSystem = flowsheet.AvailableSystemsOfUnits.Where((x) => x.Name == avunits[sender.SelectedIndex]).FirstOrDefault();
            });

            var nformats = new[] { "F", "G", "G2", "G4", "G6", "G8", "G10", "N", "N2", "N4", "N6", "R", "E", "E1", "E2", "E3", "E4", "E6" };

            c.CreateAndAddLabelRow(dl, "Number Formats");

            c.CreateAndAddDropDownRow(dl, "General", nformats.ToList(), nformats.ToList().IndexOf(flowsheet.FlowsheetOptions.NumberFormat), (sender, e) =>
            {
                flowsheet.FlowsheetOptions.NumberFormat = sender.SelectedValue.ToString();
            });

            c.CreateAndAddDescriptionRow(dl, "Select the formatting scheme for general numbers.");

            c.CreateAndAddDropDownRow(dl, "Compound Amounts", nformats.ToList(), nformats.ToList().IndexOf(flowsheet.FlowsheetOptions.FractionNumberFormat), (sender, e) =>
            {
                flowsheet.FlowsheetOptions.FractionNumberFormat = sender.SelectedValue.ToString();
            });

            c.CreateAndAddDescriptionRow(dl, "Select the formatting scheme for compound amounts in Material Stream reports.");

            page.ContentContainer.Add(dl);
            page.ShowModal();
        
        }

        private void SetupPropertyPackage()
        {

            flowsheet.PropertyPackages.Clear();

            if (hasLowPressure) {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["Ideal (Raoult's Law)"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

            if (hasHC | hasHCW)
            {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["Peng-Robinson (PR)"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

            if (hasPolarChemicals)
            {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["NRTL"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

            if (hasRefrigeration)
            {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["CoolProp"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

            if (hasSingleCompoundWater)
            {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["Steam Tables (IAPWS-IF97)"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

            if (hasElectrolytes)
            {
                var pp = (PropertyPackage)flowsheet.AvailablePropertyPackages["Extended UNIQUAC (Aqueous Electrolytes)"].Clone();
                pp.UniqueID = Guid.NewGuid().ToString();
                pp.Tag = pp.ComponentName + " (" + (flowsheet.PropertyPackages.Count + 1).ToString() + ")";
                flowsheet.AddPropertyPackage(pp);
                return;
            }

        }

        private void SetupFlashAlgorithm()
        {

            flowsheet.FlowsheetOptions.FlashAlgorithms.Clear();

            //private bool hasHCW = false;
            if (hasHCW && hasTwoLiquids)
            {
                var fa = (IFlashAlgorithm)flowsheet.AvailableFlashAlgorithms["Nested Loops (VLLE)"].Clone();
                fa.Tag = fa.Name + " (" + (flowsheet.FlowsheetOptions.FlashAlgorithms.Count + 1).ToString() + ")";
                fa.FlashSettings[Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity] = "2"; 
                flowsheet.FlowsheetOptions.FlashAlgorithms.Add(fa);
                return;
            }else if (hasHCWI)
            {
                var fa = (IFlashAlgorithm)flowsheet.AvailableFlashAlgorithms["Nested Loops (Immiscible VLLE)"].Clone();
                fa.Tag = fa.Name + " (" + (flowsheet.FlowsheetOptions.FlashAlgorithms.Count + 1).ToString() + ")";
                fa.FlashSettings[Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestCompIds] = "Water";
                flowsheet.FlowsheetOptions.FlashAlgorithms.Add(fa);
                return;
            }else if (hasSolids)
            {
                var fa = (IFlashAlgorithm)flowsheet.AvailableFlashAlgorithms["Nested Loops (SVLE - Eutectic)"].Clone();
                fa.Tag = fa.Name + " (" + (flowsheet.FlowsheetOptions.FlashAlgorithms.Count + 1).ToString() + ")";
                fa.FlashSettings[Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestCompIds] = "Water";
                flowsheet.FlowsheetOptions.FlashAlgorithms.Add(fa);
                return;
            }
            else if (hasTwoLiquids)
            {
                var fa = (IFlashAlgorithm)flowsheet.AvailableFlashAlgorithms["Nested Loops (VLLE)"].Clone();
                fa.Tag = fa.Name + " (" + (flowsheet.FlowsheetOptions.FlashAlgorithms.Count + 1).ToString() + ")";
                fa.FlashSettings[Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity] = "2";
                flowsheet.FlowsheetOptions.FlashAlgorithms.Add(fa);
                return;
            }

        }

    }
}
