using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;
using DateTime = System.DateTime;
using CultureInfo = System.Globalization.CultureInfo;

namespace Tests
{
    public class SaveSlotControllerTests
    {
        protected string pathToScene = "Prefabs/SaveSlotControllerTests";
        protected GameObject scenePrefab;
        protected GameObject sceneObj;

        [SetUp]
        public void Setup()
        {
            scenePrefab = Resources.Load<GameObject>(pathToScene);
            sceneObj = Object.Instantiate(scenePrefab);
            slots = new List<SaveSlotController>(GameObject.FindObjectsOfType<SaveSlotController>());
            slots.Sort(SortSlotsAscending);
            AssignMetasToSlots();
        }

        protected List<SaveSlotController> slots;

        protected int SortSlotsAscending(SaveSlotController firstSlot, SaveSlotController secondSlot)
        {
            var firstSlotNum = firstSlot.transform.GetSiblingIndex();
            var secondSlotNum = secondSlot.transform.GetSiblingIndex();

            if (firstSlotNum > secondSlotNum)
                return 1;
            else if (firstSlotNum < secondSlotNum)
                return -1;
            else
                return 0;
        }

        protected void AssignMetasToSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var currentSlot = slots[i];
                var currentMeta = metasToAssign[i];
                currentSlot.LinkedMeta = currentMeta;
            }
        }

        protected SaveGameMetaData[] metasToAssign = new SaveGameMetaData[]
        {
            new SaveGameMetaData
            {
                description = "First desc",
                lastWritten = DateTime.Parse("1/2/2010 4:36:12 PM"),
            },

            new SaveGameMetaData
            {
                description = "Second desc",
                lastWritten = DateTime.Parse("2/4/2015 12:34:45 PM"),
            },

            new SaveGameMetaData
            {
                description = "Third desc's the charm",
                lastWritten = DateTime.Parse("5/12/2018 7:30:21 AM"),
            },
        };

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(sceneObj.gameObject);
        }

        [Test]
        public void NumberViewsRegistered()
        {
            // We want to be sure that the slots have views for the slot number, description and date
            foreach (var slotEl in slots)
            {
                var numberView = slotEl.GetView<SaveSlotNumberView>();
                Assert.NotNull(numberView);
            }

        }

        [Test]
        public void DescViewsRegistered()
        {
            // We want to be sure that the slots have views for the slot number, description and date
            foreach (var slotEl in slots)
            {
                var descView = slotEl.GetView<SaveDescriptionView>();
                Assert.NotNull(descView);
            }

        }

        [Test]
        public void DateViewsRegistered()
        {
            // We want to be sure that the slots have views for the slot number, description and date
            foreach (var slotEl in slots)
            {
                var dateView = slotEl.GetView<SaveDateView>();
                Assert.NotNull(dateView);
            }

        }

        [Test]
        public void CorrectNumbersDisplayed()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var currentSlot = slots[i];

                var displayerComponent = currentSlot.GetComponentInChildren<SaveSlotNumberView>();
                var slotNumber = displayerComponent.SlotNumber;
                
                var expected = displayerComponent.Prefix + slotNumber + displayerComponent.Postfix;
                var textDisplayed = displayerComponent.Text;
                var displaysCorrectNumber = textDisplayed == expected;

                Assert.IsTrue(displaysCorrectNumber);
            }
        }

        [Test]
        public void CorrectDescsDisplayed()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var currentSlot = slots[i];
                var currentMeta = metasToAssign[i];

                var displayerComponent = currentSlot.GetComponentInChildren<SaveDescriptionView>();
                
                var expected = currentMeta.description;
                var textDisplayed = displayerComponent.Text;
                var displaysCorrectNumber = textDisplayed == expected;

                Assert.IsTrue(displaysCorrectNumber);
            }
        }

        [Test]
        public void CorrectDatesDisplayed()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var currentSlot = slots[i];
                var currentMeta = metasToAssign[i];

                var displayerComponent = currentSlot.GetComponentInChildren<SaveDateView>();

                // We want to be sure that the text is being displayed in the format specified by the view
                var dateFormat = displayerComponent.Format;

                var expected = currentMeta.lastWritten.ToString(dateFormat, localCulture);
                var textDisplayed = displayerComponent.Text;
                var displaysCorrectDate = textDisplayed == expected;

                Assert.IsTrue(displaysCorrectDate);
            }
        }

        protected CultureInfo localCulture = CultureInfo.CurrentUICulture;
    }
}
