using Godot;
using System;

public partial class Brick : StaticBody2D, IPoolable
{
	[Export] private int width = 60;	
	[Export] private int height = 20;
	[Export] private Color color = Colors.White;
	private Sprite2D _sprite;
	private CollisionShape2D _collision;


	public void OnSpawn()
	{
		if (_collision != null)
		{
			_collision.Disabled = false;
			GD.Print($"Brick spawned at {Position}, CollisionLayer: {CollisionLayer}, CollisionShape disabled: {_collision.Disabled}");
		}
		Visible = true;
	}

	public void OnDespawn()
	{
		Visible = false;
		if (_collision != null)
		{
			_collision.Disabled = true;
			GD.Print($"Brick despawned at {Position}, CollisionShape disabled: {_collision.Disabled}");
		}
	}

	public void SetColor(Color c)
	{
		color = c;
		_sprite.Texture = CreateBrickTexture(width, height, color);
	}

	public void Hit()
	{
		// Logic for when the brick is hit by a ball
		GD.Print("Brick hit!");
		ObjectPoolReturnHandler<Brick> brickReturnHandler = GetParent() as ObjectPoolReturnHandler<Brick>;
		brickReturnHandler?.Return(this);
		OnDespawn();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionLayer = 1;

		_sprite = new Sprite2D();
		AddChild(_sprite);
		_sprite.Texture = CreateBrickTexture(width, height, color);

		_collision = new CollisionShape2D();
		AddChild(_collision);
		var shape = new RectangleShape2D();
		shape.Size = new Vector2(width, height);
		_collision.Shape = shape;
		_collision.Disabled = true;
	}

	private ImageTexture CreateBrickTexture(int width, int height, Color color)
	{
		var image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				image.SetPixel(x, y, color);
			}
		}

		var texture = new ImageTexture();
		texture.SetImage(image);
		return texture;
	}
}