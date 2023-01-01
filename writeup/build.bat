@echo off

echo Preparing to compile writeup...
echo Utility script by Rubens Pirie

DEL main.pdf
echo Deleted old main.pdf

if "%1" == "full" (
  echo Running a full compilation of PDF this may take some time...
  echo Pass 1
  pdflatex main.tex -shell-escape -interaction=nonstopmode > nul

  echo Pass 2
  pdflatex main.tex -shell-escape -interaction=nonstopmode > nul

) else (
  echo Running a partial compilation of PDF run script with "full" as the first argument to run a full compilation.

  echo Pass 1
  pdflatex main.tex -shell-escape -interaction=nonstopmode > nul

)

echo Done.