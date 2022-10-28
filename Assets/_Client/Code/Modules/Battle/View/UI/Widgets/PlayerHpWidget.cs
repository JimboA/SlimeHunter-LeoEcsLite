using JimboA.Plugins.EcsProviders;
using Leopotam.EcsLite;
using JimboA.Plugins.Tween;
using Unity.Mathematics;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Client.Battle.View.UI
{
    public class PlayerHpWidget : WidgetBase, IStatView<int>
    {
        [SerializeField] private GameObject hpIconPrefab;
        [SerializeField] private float heartDestroyingTime;
        [SerializeField] private float heartDestroyingScale;

        public void OnInit(int amount, EcsWorld world)
        {
            OnUpdate(amount, world);
        }

        public void OnUpdate(int amount, EcsWorld world)
        {
            if (amount <= 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    KillHeartAnimationPlay(child.gameObject, world);
                }
                return;
            }
            
            var dif = amount - transform.childCount;
            if(dif == 0) return;
            
            var len = math.abs(dif);
            if (dif < 0)
            {
                for (int i = 0; i < len; i++)
                {
                    var index = transform.childCount - 1;
                    if(index < 0) break;
                    var child = transform.GetChild(index);
                    KillHeartAnimationPlay(child.gameObject, world);
                }
            }
            else
            {
                var tr = transform;
                for (int i = 0; i < len; i++)
                {
                    var heartViewTr = world.CreateView(hpIconPrefab, tr.position, tr.rotation).transform;
                    heartViewTr.SetParent(tr);
                    heartViewTr.localScale = Vector3.one;
                }
            }
        }

        private void KillHeartAnimationPlay(GameObject heart, EcsWorld world)
        {
            var tr = heart.transform;
            var scale = tr.localScale;
            tr.DoScale(world, scale, scale * heartDestroyingScale, heartDestroyingTime);
            var heartImage = heart.GetComponent<Image>();
            heartImage.DoFade(world, 1, 0, heartDestroyingTime);
            Destroy(heart, heartDestroyingTime);
        }
    }
}