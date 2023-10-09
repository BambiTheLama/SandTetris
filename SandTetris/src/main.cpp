#include "raylib.h"

int main()
{
	InitWindow(1600, 900, "Sand Tetris BB/SzK");
	while (!WindowShouldClose())
	{
		BeginDrawing();
		ClearBackground(BLUE);
		EndDrawing();
	}
	CloseWindow();
	return 0;
}