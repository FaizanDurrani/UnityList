using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Base36Encoder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class UnityListItem
    {
        private readonly UnityListSearchApiResultEntry _entry;
        private readonly VisualElement _parent;
        private readonly Texture2D _emptyTexture = Resources.Load<Texture2D>("defaultImage");

        public UnityListItem(UnityListSearchApiResultEntry entry, ref ScrollView parent)
        {
            _entry = entry;
            _parent = parent;

            BuildUi();
        }

        private void BuildUi()
        {
            var itemContainer = new VisualContainer();
            itemContainer.AddToClassList("itemContainer");

            var image = new Image{image = StyleValue<Texture>.Create(_emptyTexture)};
            image.AddToClassList("itemImage");
            itemContainer.Add(image);

            var itemDetails = new VisualContainer();
            itemDetails.AddToClassList("itemDetails");
            itemContainer.Add(itemDetails);

            var title = new Label(_entry.Title);
            title.AddToClassList("itemTitle");
            itemDetails.Add(title);

            var authorAndUpdatedAt =
                new Label(_entry.Author + " - Updated At: " + _entry.UpdatedAt.ToShortDateString());
            authorAndUpdatedAt.AddToClassList("itemAuthor");
            itemDetails.Add(authorAndUpdatedAt);

            var description = new Label(_entry.Description);
            description.AddToClassList("itemDescription");
            itemDetails.Add(description);

            var actionButtons = new VisualContainer();
            actionButtons.AddToClassList("itemActionButtons");
            itemDetails.Add(actionButtons);

            var downloadButton = new Button(Download) {text = "Download"};
            downloadButton.AddToClassList("itemDownloadButton");
            actionButtons.Add(downloadButton);

            var itemViewButton = new Button(ViewItem) {text = "Open in Browser"};
            itemViewButton.AddToClassList("itemViewButton");
            actionButtons.Add(itemViewButton);

            _parent.Add(itemContainer);

            LoadImage(image);
        }

        private void ViewItem()
        {
            Application.OpenURL("https://unitylist.com/p/" + Base36.Encode(0 | _entry.Id) + "/" + _entry.Slug);
        }

        private void Download()
        {
            var path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, _entry.Title);
            Debug.Log(path);
            EditorUtility.ClearProgressBar();
            if (path.Length <= 1) return;
            UnityListEditor.Download(_entry.Title + ".zip", path + "/" + _entry.Title + ".zip", "http://unitylist.com/api/get?download&id=" + _entry.Id);
        }

        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Debug.Log("Done");
        }

        private void LoadImage(Image image)
        {
            if (_entry.Image == null) return;
            
            UnityWebRequest texture = UnityWebRequestTexture.GetTexture(_entry.Image);
            var asyncReq = texture.SendWebRequest();
            asyncReq.completed += operation =>
            {
                if (asyncReq.isDone)
                {
                    var tex = DownloadHandlerTexture.GetContent(texture);
                    image.image = StyleValue<Texture>.Create(tex);
                }
            };
        }
    }
}