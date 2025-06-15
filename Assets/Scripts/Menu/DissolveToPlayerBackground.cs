namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class DissolveToPlayerBackground : MonoBehaviour
    {
    	Material material;
    
        [SerializeField]
    	private bool _isDissolving = false;
    	[SerializeField]
    	private float _fade = 1f;
    	[SerializeField]
    	private float _dissolveSpeed = 2;
    	[SerializeField]
    	private float _reappearSpeed = 2;
    
    	[SerializeField]
    	private Vector2 _p3existencePos;
    	private Vector2 _initPos;
    
    	void Start()
    	{
    		// Get a reference to the material
    		_initPos = transform.position;
    		material = GetComponent<SpriteRenderer>().material;
    		material.SetFloat("_Fade", _fade);
    	}
    
    	void Update()
    	{
    		if (_isDissolving)
    		{
    			_fade -= Time.deltaTime * _dissolveSpeed;
    
    			if (_fade <= 0f)
    				_fade = 0f;
    
    		}
    		else
    		{
    			_fade += Time.deltaTime * _reappearSpeed;
    
    			if (_fade >= 1f)
    				_fade = 1f;
    		}
    		// Set the property
    		material.SetFloat("_Fade", _fade);
    	}
    
    	public void SetDissolve(bool val)
        {
    		_isDissolving = val;
    	}
    	public void SetFadeVal(float val)
    	{
    		_fade = val;
    	}
    	public void P3Mode(bool val)
    	{
    		if (val)
    			transform.position = _p3existencePos;
    		else
    			transform.position = _initPos;
    	}
    }
}
