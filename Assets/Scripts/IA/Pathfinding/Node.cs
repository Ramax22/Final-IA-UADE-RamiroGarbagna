using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neightbourds;
    public bool isTrap;

    void GetNeightbourd(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, 2.2f))
        {
            neightbourds.Add(hit.collider.GetComponent<Node>());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (neightbourds.Count != 0)
        {
            foreach (var item in neightbourds)
            {
                //Vector3 line_finish = item.transform.position;
                //Gizmos.DrawLine(transform.position, line_finish);
                //Vector3 direction = (line_finish - transform.position).normalized;
                ForGizmo(transform.position, item.transform.position - transform.position, Color.green, false, 1f);
            }
        }
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    public void ForGizmo(Vector3 pos, Vector3 direction, Color? color = null, bool doubled = false, float arrowHeadLength = 0.2f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color ?? Color.white;

        //arrow shaft
        Gizmos.DrawRay(pos, direction);

        if (direction != Vector3.zero)
        {
            //arrow head
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
    }
}