from z3 import *

set_param("solver.proof.check", True)

def render_expr(e):
    if is_const(e) and e.num_args() == 0:
        return str(e)

    name = e.decl().name()

    if name == "not":
        return f"Not({render_expr(e.arg(0))})"

    if name == "and":
        return "And(" + ", ".join(render_expr(c) for c in e.children()) + ")"

    if name == "or":
        return "Or(" + ", ".join(render_expr(c) for c in e.children()) + ")"

    if name == "=":
        return f"=({render_expr(e.arg(0))}, {render_expr(e.arg(1))})"

    return f"{name}(" + ", ".join(render_expr(c) for c in e.children()) + ")"


def format_arg(a):

    if isinstance(a, AstVector):
        return "[" + ", ".join(render_expr(x) for x in a) + "]"

    if is_expr(a):
        return render_expr(a)

    return str(a)


def show_clause(*args):
    print(" ".join(format_arg(a) for a in args))


s = Solver()
onc = OnClause(s, show_clause)

s.from_file("LevelCrossingExample_p_step.smt2")
#print(s.check())