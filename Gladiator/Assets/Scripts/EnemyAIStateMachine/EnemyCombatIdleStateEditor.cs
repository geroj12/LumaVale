using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyCombatIdleState))]
public class EnemyCombatIdleStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Referenz
        EnemyCombatIdleState state = (EnemyCombatIdleState)target;

        // Default Inspector (zeigt die Slider und Felder an)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📊 Calculated Probabilities", EditorStyles.boldLabel);

        // Gesamtgewicht nur von Aktionen, die überhaupt ein Weight > 0 haben
        int total = 0;
        if (state.attack.weight > 0) total += state.attack.weight;
        if (state.block.weight > 0) total += state.block.weight;
        if (state.dodge.weight > 0) total += state.dodge.weight;
        if (state.retreat.weight > 0) total += state.retreat.weight;

        if (total > 0)
        {
            ShowProbability("Attack", state.attack.weight, state.attack.cooldown, total);
            ShowProbability("Block", state.block.weight, state.block.cooldown, total);
            ShowProbability("Dodge", state.dodge.weight, state.dodge.cooldown, total);
            ShowProbability("Retreat", state.retreat.weight, state.retreat.cooldown, total);
        }
        else
        {
            EditorGUILayout.HelpBox("Keine Weights gesetzt! Alle Wahrscheinlichkeiten = 0.", MessageType.Warning);
        }
    }

    private void ShowProbability(string label, int weight, float cooldown, int total)
    {
        if (weight <= 0) return; // ignorieren, wenn Aktion deaktiviert

        float percent = (weight / (float)total) * 100f;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{label}:", GUILayout.Width(70));
        EditorGUILayout.LabelField($"{percent:0.0} %", GUILayout.Width(60));
        EditorGUILayout.LabelField($"Cooldown: {cooldown:0.0}s");
        EditorGUILayout.EndHorizontal();
    }
}
