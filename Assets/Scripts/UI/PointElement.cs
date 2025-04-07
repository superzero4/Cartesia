using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PointElement : Element<Point>
    {
        [SerializeField] private Text[] _xyz;

        public override void RefreshView()
        {
            if (_xyz.Length != 3)
            {
                return;
            }

            _xyz[0].text = Data.x.ToString();
            _xyz[1].text = Data.y.ToString();
            _xyz[2].text = Data.z.ToString();
        }

        public override void ToggleVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}