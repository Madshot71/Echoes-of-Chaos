using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

/// <summary>
/// Manages a UI bar that can be split into a variable number of segments.
/// Requires a container with a HorizontalLayoutGroup to automatically space the segments.
/// </summary>
public class SegmentedProgressBar : MonoBehaviour
{
    [Tooltip("The prefab for each segment of the bar. Must have an Image component.")]
    public Image segmentPrefab;
    public TMP_Text valueTxt;

    [Tooltip("The container where the segments will be placed. Should have a HorizontalLayoutGroup.")]
    public RectTransform segmentContainer;

    [Tooltip("The color for a segment that is highlighted or active.")]
    public Color highlightedColor = Color.white;

    [Tooltip("The color for a segment that is not highlighted or inactive.")]
    public Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

    private readonly List<Image> _segments = new List<Image>();

    /// <summary>
    /// Clears existing segments and creates a new set.
    /// </summary>
    /// <param name="count">The number of segments to create.</param>
    public void CreateSegments(int count)
    {
        // Clear any old segments
        foreach (Transform child in segmentContainer)
        {
            Destroy(child.gameObject);
        }
        _segments.Clear();

        if (count <= 0 || segmentPrefab == null) return;

        // Create new segments
        for (int i = 0; i < count; i++)
        {
            Image segmentImage = Instantiate(segmentPrefab, segmentContainer);
            
            if (segmentImage != null)
            {
                segmentImage.color = defaultColor;
                _segments.Add(segmentImage);
            }
            else
            {
                Debug.LogError("Segment prefab is missing an Image component.", this);
            }
        }
    }

    /// <summary>
    /// Highlights a single segment and de-highlights others.
    /// </summary>
    /// <param name="index">The index of the segment to highlight. De-highlights all if out of range.</param>
    public void HighlightSegment(int index , Func<string> value)
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            if (_segments[i] != null)
            {
                _segments[i].color = (i == index) ? highlightedColor : defaultColor;
            }
        }

        valueTxt.text = value();        
    }

    /// <summary>
    /// Highlights all segments up to and including the given index.
    /// </summary>
    /// <param name="index">The index of the last segment to highlight.</param>
    public void HighlightUpto(int index)
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            if (_segments[i] != null)
            {
                _segments[i].color = (i <= index) ? highlightedColor : defaultColor;
            }
        }
    }

    /// <summary>
    /// Resets all segments to the default color.
    /// </summary>
    public void ResetHighlights()
    {
        foreach (Image segment in _segments)
        {
            if (segment != null)
            {
                segment.color = defaultColor;
            }
        }
    }
}