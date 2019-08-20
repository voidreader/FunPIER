using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;

using System.IO;

[CustomEditor(typeof(AudioAssistant))]
public class AudioAssistantEditor : Editor {

    AudioAssistant main;
    AudioAssistant.Sound edit = null;


    AnimBool iapsFade = new AnimBool(false);
    AnimBool tracksFade = new AnimBool(false);

    public override void OnInspectorGUI() {
        if (!target) {
            EditorGUILayout.HelpBox("AudioAssistant is missing", MessageType.Error);
            return;
        }
        main = (AudioAssistant)target;
        Undo.RecordObject(main, "");

        

        if (main.tracks == null) main.tracks = new List<AudioAssistant.Sound>();
        if (main.sounds == null) main.sounds = new List<AudioAssistant.Sound>();
        
        #region Music Tracks
        tracksFade.target = GUILayout.Toggle(tracksFade.target, "Music Tracks", EditorStyles.foldout);

        if (EditorGUILayout.BeginFadeGroup(tracksFade.faded)) {
            EditorGUILayout.BeginVertical(EditorStyles.textArea);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Name", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(200));
            GUILayout.Label("Audio Clip", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            //foreach (AudioAssistant.MusicTrack track in main.tracks) {
            //    EditorGUILayout.BeginHorizontal();

            //    if (GUILayout.Button("X", GUILayout.Width(20))) {
            //        main.tracks.Remove(track);
            //        break;
            //    }
            //    track.name = EditorGUILayout.TextField(track.name, GUILayout.Width(100));
            //    track.tracks = (AudioClip) EditorGUILayout.ObjectField(track.tracks, typeof(AudioClip), false, GUILayout.ExpandWidth(true));

            //    EditorGUILayout.EndHorizontal();
            //}

            foreach (AudioAssistant.Sound clip in main.tracks) {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    main.sounds.Remove(clip);
                    break;
                }
                if (GUILayout.Button("Edit", GUILayout.Width(40))) {
                    if (edit == clip)
                        edit = null;
                    else
                        edit = clip;
                }

                clip.name = EditorGUILayout.TextField(clip.name, GUILayout.Width(120));

                if (edit == clip || clip.clips.Count == 0) {
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < clip.clips.Count; i++) {
                        clip.clips[i] = (AudioClip) EditorGUILayout.ObjectField(clip.clips[i], typeof(AudioClip), false, GUILayout.ExpandWidth(true));
                        if (clip.clips[i] == null) {
                            clip.clips.RemoveAt(i);
                            break;
                        }
                    }
                    AudioClip new_clip = (AudioClip) EditorGUILayout.ObjectField(null, typeof(AudioClip), false, GUILayout.Width(150));
                    if (new_clip)
                        clip.clips.Add(new_clip);
                    EditorGUILayout.EndVertical();
                } else {
                    GUILayout.Label(clip.clips.Count.ToString() + " audio clip(s)", EditorStyles.miniBoldLabel);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Width(60)))
                main.tracks.Add(new AudioAssistant.Sound());
          
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
        #endregion

        #region Sounds
        iapsFade.target = GUILayout.Toggle(iapsFade.target, "Sounds", EditorStyles.foldout);

        if (EditorGUILayout.BeginFadeGroup(iapsFade.faded)) {
            EditorGUILayout.BeginVertical(EditorStyles.textArea);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Edit", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(40));
            GUILayout.Label("Name", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(300));
            GUILayout.Label("Audio Clips", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            foreach (AudioAssistant.Sound clip in main.sounds) {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    main.sounds.Remove(clip);
                    break;
                }
                if (GUILayout.Button("Edit", GUILayout.Width(40))) {
                    if (edit == clip)
                        edit = null;
                    else
                        edit = clip;
                }

                clip.name = EditorGUILayout.TextField(clip.name, GUILayout.Width(250));

                if (edit == clip || clip.clips.Count == 0) {
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < clip.clips.Count; i++) {
                        clip.clips[i] = (AudioClip) EditorGUILayout.ObjectField(clip.clips[i], typeof(AudioClip), false, GUILayout.ExpandWidth(true));
                        if (clip.clips[i] == null) {
                            clip.clips.RemoveAt(i);
                           break;
                        }
                    }
                    AudioClip new_clip = (AudioClip) EditorGUILayout.ObjectField(null, typeof(AudioClip), false, GUILayout.Width(150));
                    if (new_clip)
                        clip.clips.Add(new_clip);
                    EditorGUILayout.EndVertical();
                } else {
                    GUILayout.Label(clip.clips.Count.ToString() + " audio clip(s)", EditorStyles.miniBoldLabel);
                }


                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Width(60))) {
                main.sounds.Add(new AudioAssistant.Sound());
                edit = main.sounds[main.sounds.Count - 1];
            }
            if (GUILayout.Button("Sort", GUILayout.Width(60))) {
                main.sounds.Sort((AudioAssistant.Sound a, AudioAssistant.Sound b) => {
                    return string.Compare(a.name, b.name);
                });
                foreach (AudioAssistant.Sound sound in main.sounds)
                    sound.clips.Sort((AudioClip a, AudioClip b) => {
                        return string.Compare(a.ToString(), b.ToString());
                    });
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
        #endregion
    }


}
