using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

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
            slots = GameObject.FindObjectsOfType<SaveSlotController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(sceneObj.gameObject);
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

        protected SaveSlotController[] slots;

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

    }
}
