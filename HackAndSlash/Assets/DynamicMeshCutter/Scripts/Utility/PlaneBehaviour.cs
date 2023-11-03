
namespace DynamicMeshCutter
{
    public class PlaneBehaviour : CutterBehaviour
    {
        public float DebugPlaneLength = 2;
        public void Cut(UnityEngine.GameObject[] roots)
        {
            foreach (var root in roots)
            {
                if (!root.activeInHierarchy)
                    continue;
                var targets = root.GetComponentsInChildren<MeshTarget>();
                foreach (var target in targets)
                {
                    target.canCut = false;
                    Cut(target, transform.position, transform.forward, null, OnCreated);
                }
            }
        }
        public void Cut()
        {
          
        }
        void OnCreated(Info info, MeshCreationData cData)
        {
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
        }


    }
}