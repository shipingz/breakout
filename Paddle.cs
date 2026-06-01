using Godot;
using System;

public partial class Paddle : CharacterBody2D
{
	[Export] private float speed = 500.0f;
	[Export] private float width = 300f;
	[Export] private float height = 20f;
	private Sprite2D _sprite;
	private CollisionShape2D _collision;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionLayer = 1;
		CollisionMask = 1;

		_sprite = new Sprite2D();
		AddChild(_sprite);
		_sprite.Texture = CreatePaddleTexture((int)width, (int)height, Colors.White);

		_collision = new CollisionShape2D();
		AddChild(_collision);
		var shape = new RectangleShape2D();
		shape.Size = new Vector2(width, height);
		_collision.Shape = shape;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X += speed;
		}
		if (Input.IsActionPressed("ui_left"))
		{
			velocity.X -= speed;
		}

		Velocity = velocity.Normalized() * 400;
		MoveAndSlide();
	}	

	public Vector2 HitByBall(Ball ball)
	{
		float hitOffset = ball.Position.X - Position.X;
		float normalizedOffset = hitOffset / (width / 2);
		normalizedOffset = Mathf.Clamp(normalizedOffset, -1, 1);

		float maxBounceAngle = Mathf.DegToRad(75);
		float bounceAngle = normalizedOffset * maxBounceAngle;

		return new Vector2(Mathf.Sin(bounceAngle), -Mathf.Cos(bounceAngle)).Normalized();
	}

	private ImageTexture CreatePaddleTexture(int width, int height, Color color)
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