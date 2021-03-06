﻿using System;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Editor
{
    public class UnityListEditor : EditorWindow
    {
        [SerializeField] private static UnityListEditor _window;

        [SerializeField] private VisualElement _root;
        [SerializeField] private ScrollView _list;
        [SerializeField] private UnityListSearchApi _currentReq;

        [MenuItem("Tools/UnityList")]
        private static void GetWindow()
        {
            _window = GetWindow<UnityListEditor>();
            _window.Show();
        }

        private void Awake()
        {
            BuildUi();
        }

        private void OnEnable()
        {
            RebuildUi();
        }

        private void BuildUi()
        {
            _root = this.GetRootVisualContainer();
            _root.AddStyleSheetPath("stylesheet");

            BuildTopMenu();
            BuildList();

            GetFeaturedItems();
        }

        private void GetFeaturedItems()
        {
            _currentReq = new UnityListSearchApi(null);
            _currentReq.Search((items) =>
            {
                foreach (var item in items)
                {
                    new UnityListItem(item, ref _list);
                }
            });
        }

        private void BuildList()
        {
            //if (_list != null) return;

            _list = new ScrollView {name = "itemsList", stretchContentWidth = true};
            _list.verticalScroller.valueChanged += e =>
            {
                var listVerticalScroller = _list.verticalScroller;
                if (Math.Abs(e - listVerticalScroller.highValue) < 0.001f)
                {
                    if (_currentReq != null && _currentReq.HasAnotherPage)
                        _currentReq.NextPage(DisplayItems);
                }
            };
            _root.Add(_list);
        }

        private void BuildTopMenu()
        {
            var menuContainer = new VisualContainer {name = "topMenu"};
            var label = new Label("UnityList");
            var searchInput = new TextField {name = "searchInput"};
            var searchButton = new Button(Search) {name = "search", text = "SEARCH"};

            menuContainer.Add(label);
            menuContainer.Add(searchInput);
            menuContainer.Add(searchButton);
            _root.Add(menuContainer);
        }

        private void Search()
        {
            var searchInput = _root.Q("topMenu").Q<TextField>("searchInput").text.Trim();
            Debug.Log(searchInput);

            _currentReq = new UnityListSearchApi(searchInput);
            _currentReq.Search((i) =>
            {
                _list.Clear();
                foreach (var item in i)
                {
                    new UnityListItem(item, ref _list);
                }
            });
        }

        private void DisplayItems(UnityListSearchApiResultEntry[] items)
        {
            foreach (var item in items)
            {
                new UnityListItem(item, ref _list);
            }
        }

        private static void OnProjectChange()
        {
            if (_window != null)
                _window.RebuildUi();
        }

        private void RebuildUi()
        {
            if (_root != null) _root.Clear();
            _root = null;

            BuildUi();
        }

        private static UnityListFile _downloadOperation;

        public static void Download(string fileName, string path, string url)
        {
            _downloadOperation = new UnityListFile(fileName, url, path);
        }

        private void OnInspectorUpdate()
        {
            if (_downloadOperation == null || _downloadOperation.Completed)
            {
                EditorUtility.ClearProgressBar();
                _downloadOperation = null;
            }
            else
            {
                var percentage = (int) (_downloadOperation.Progress * 100) + "%";
                var info = "Downloading " + _downloadOperation.FileName + " " + _downloadOperation.Downloaded + "/" +
                           _downloadOperation.FileSize + "KB";
                if (
                    EditorUtility.DisplayCancelableProgressBar("Downloading " + percentage, info,
                        _downloadOperation.Completed ? 1f : _downloadOperation.Progress))
                {
                    _downloadOperation.Cancel();
                    _downloadOperation = null;
                }
            }
        }
    }
}