export interface Todo {
    id: number;
    title: string;
    isCompleted: boolean;
    createdAt: string;
    completedAt: string | null;
}

export interface TodoUpdate {
    title: string;
    isCompleted: boolean;
}
