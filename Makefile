# Re.Shop root Makefile — action/package dispatcher.
#
#   make                     show this help
#   make <action>            run an action across the whole repo
#   make <action> <package>  run an action against one package
#
# Packages: api | admin | storefront | all (default)

ACTIONS  := install tools setup build test lint type-check format readme-check check run outdated clean
PACKAGES := api admin storefront all

.DEFAULT_GOAL := help

# /usr/share/dotnet is not on PATH in this environment. Prepend it only when
# dotnet is otherwise unresolvable, so this is a no-op on a normal setup.
ifeq ($(shell command -v dotnet 2>/dev/null),)
export PATH := /usr/share/dotnet:$(PATH)
endif

.PHONY: $(ACTIONS) $(PACKAGES) help validate-target

# Every action is one script under scripts/. The Makefile holds no project
# logic — it validates the package, then hands the rest to the script.
$(ACTIONS): validate-target
	@scripts/$@.sh $(filter-out $@,$(MAKECMDGOALS))

# Declaring packages as (empty) goals lets Make reject unknown names itself.
$(PACKAGES):
	@:

validate-target:
	@action='$(firstword $(MAKECMDGOALS))'; \
	pkg='$(filter-out $(firstword $(MAKECMDGOALS)),$(MAKECMDGOALS))'; \
	pkg=$${pkg:-all}; \
	case " $(ACTIONS) " in *" $$action "*) ;; \
	  *) echo "error: unknown action '$$action'" >&2; \
	     echo "usage: make <action> [package]" >&2; exit 1 ;; esac; \
	case " $(PACKAGES) " in *" $$pkg "*) ;; \
	  *) echo "error: unknown package '$$pkg'" >&2; \
	     echo "valid packages: $(PACKAGES)" >&2; \
	     echo "usage: make <action> [package]" >&2; exit 1 ;; esac

help:
	@echo "usage: make <action> [package]"
	@echo ""
	@echo "actions:"
	@echo "  install       pnpm install (workspace-wide)"
	@echo "  tools         dotnet tool restore"
	@echo "  setup         install + tools + dotnet restore"
	@echo "  build         build .NET and the frontend"
	@echo "  test          run .NET and frontend tests"
	@echo "  lint          verify formatting (.NET) and lint the frontend"
	@echo "  type-check    type-check the frontend"
	@echo "  format        apply formatting (.NET and frontend)"
	@echo "  readme-check  verify folder READMEs against guide/folder-readme-guide.md"
	@echo "  check         build -> test -> lint -> type-check -> readme-check"
	@echo "  run           start a package (no package = full Aspire stack)"
	@echo "  outdated      show NuGet and npm dependency drift"
	@echo "  clean         remove build output (keeps node_modules)"
	@echo ""
	@echo "packages: api admin storefront all"
	@echo ""
	@echo "examples:"
	@echo "  make check"
	@echo "  make build api"
	@echo "  make test storefront"
	@echo "  make run"
