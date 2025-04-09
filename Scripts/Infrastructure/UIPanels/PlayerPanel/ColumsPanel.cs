using System.Collections.Generic;
using Network.NetworkObjects;
using UnityEngine;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class ColumsPanel : MonoBehaviour
    {
        [SerializeField] private List<Colum> _colums;

        public RectTransform PlayerPosition(int index) => _colums[index].PlayerPosition;

        public void ShowColum(int index)
        {
            _colums[index].Show();
        }

        public void SetColumsStatus(List<PlayersAnswered> list)
        {
            if (list[0].Coins >= 1)
            {
               // _colums[0].SetStatus((ColumStatus)0);
                _colums[0].SetIcon(0);
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].Coins < 1)
                    {
                       // _colums[i].SetStatus((ColumStatus)3);
                        _colums[i].SetIcon(3);
                    }
                    else
                    {
                        if (list[i - 1].Coins > list[i].Coins)
                        {
                          //  _colums[i].SetStatus((ColumStatus)i);
                        }
                        else
                        {
                           // _colums[i].SetStatus((ColumStatus)i - 1);
                        }

                        _colums[i].SetIcon(i);
                    }
                }
            }
        }
    }
}