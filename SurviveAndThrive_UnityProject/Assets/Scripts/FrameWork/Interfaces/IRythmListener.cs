using UnityEngine;
using System.Collections;

/// <summary>
/// Implement this for all classes that need to listen to the rhythm
/// </summary>
public interface IRythmListener {

    void RegisterToController();

    void UnRegisterToController();

    /// <summary>
    /// Implement this for all rhythm based logic
    /// </summary>
    /// <param name="rhythmTimer"></param>
    void UpdateRhytm(float rhythmTimer, float blockLenght);
}