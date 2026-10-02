from pathlib import Path
import runpy
HERE=Path(__file__).resolve().parent
runpy.run_path(str(HERE / "generate_m5_theory_pdf.py"), run_name="__main__")
runpy.run_path(str(HERE / "generate_m5_practice_pdf.py"), run_name="__main__")
